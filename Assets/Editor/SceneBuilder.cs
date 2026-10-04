using System.Collections.Generic;
using System.IO;
using System.Linq;
using TMPro;
using UnityEditor;
using UnityEditor.Animations;
using UnityEditor.Build.Reporting;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;

// Menú "Rescate" en la barra de Unity:
//   1. Generar escenas  -> crea Menu, Nivel1_Jugueteria y Nivel2_Calle con todo conectado y las agrega a Build Profiles.
//   2. Build Windows    -> genera el .exe en la carpeta Builds/Windows.
//
// Usa tu arte si lo pones en Assets/Art/<carpeta>. Si una carpeta está vacía, usa formas de colores.
// Usa tu audio si lo pones en Assets/Audio con estos nombres: musica, golpe, llave, corazon, regalo, pasos.
// Lo que genera va a Assets/Generado (se puede regenerar las veces que quieras).
public static class SceneBuilder
{
    const string GenDir = "Assets/Generado";
    const string SpritesDir = GenDir + "/Sprites";
    const string AnimDir = GenDir + "/Animaciones";
    const string PrefabsDir = GenDir + "/Prefabs";
    const string ArtDir = "Assets/Art";
    const string AudioDir = "Assets/Audio";
    const string ScenesDir = "Assets/Scenes";
    const string MenuScenePath = ScenesDir + "/Menu.unity";
    const string Level1Name = "Nivel1_Jugueteria";
    const string Level2Name = "Nivel2_Calle";
    const string Level1Path = ScenesDir + "/" + Level1Name + ".unity";
    const string Level2Path = ScenesDir + "/" + Level2Name + ".unity";
    const string ExePath = "Builds/Windows/RescateDelConejo.exe";

    static readonly string[] ArtFolders =
    {
        "Chica/Idle", "Chica/Run", "Monstruo", "Conejo", "Llave", "Corazon", "Regalo", "Fondo/Jugueteria", "Fondo/Calle"
    };

    static readonly float[] LaneY = { 2f, 0f, -2f };
    const float CameraSize = 4f;

    static readonly List<string> report = new List<string>();

    // ------------------------------------------------------------------ menú

    [MenuItem("Rescate/1. Generar escenas (Menu + 2 niveles)")]
    static void GenerateAll()
    {
        if (Resources.Load<TMP_Settings>("TMP Settings") == null)
        {
            EditorUtility.DisplayDialog("Falta TextMeshPro",
                "Primero importa los recursos de TextMeshPro:\nWindow > TextMeshPro > Import TMP Essential Resources\n\nLuego vuelve a usar Rescate > 1. Generar escenas.",
                "OK");
            return;
        }

        bool exists = File.Exists(MenuScenePath) || File.Exists(Level1Path) || File.Exists(Level2Path);
        if (exists && !EditorUtility.DisplayDialog("Regenerar escenas",
                "Ya existen escenas del juego en Assets/Scenes (Menu / Nivel1_Jugueteria / Nivel2_Calle).\nSe van a REEMPLAZAR (los cambios hechos a mano en esas escenas se pierden).\n\n¿Continuar?",
                "Sí, regenerar", "Cancelar"))
        {
            return;
        }

        if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo()) return;

        report.Clear();
        PrepareFolders();
        FixPixelArtImport();

        var assets = new GameAssets();
        assets.Load();

        assets.MakePrefabs();

        BuildGameScene(assets, new Level
        {
            Path = Level1Path,
            Title = "NIVEL 1: LA JUGUETERÍA",
            Keys = 3,
            StartInterval = 1.5f,
            Next = Level2Name,
            WinMessage = "¡Abriste la puerta!\nEl monstruo huyó a la calle...",
            Background = assets.BgToy,
            Tile = assets.ToyTile,
            CameraColor = new Color(0.45f, 0.25f, 0.2f),
        });
        BuildGameScene(assets, new Level
        {
            Path = Level2Path,
            Title = "NIVEL 2: LA CALLE",
            Keys = 5,
            StartInterval = 1.1f,
            Next = "",
            WinMessage = "¡Liberaste al conejo!",
            Background = assets.BgStreet,
            Tile = assets.StreetTile,
            CameraColor = new Color(0.24f, 0.12f, 0.2f),   // cielo de atardecer
        });
        BuildMenuScene(assets);

        EditorBuildSettings.scenes = new[]
        {
            new EditorBuildSettingsScene(MenuScenePath, true),
            new EditorBuildSettingsScene(Level1Path, true),
            new EditorBuildSettingsScene(Level2Path, true),
        };

        EditorSceneManager.OpenScene(MenuScenePath);
        AssetDatabase.SaveAssets();

        string summary = report.Count == 0
            ? "Se usó todo tu arte y audio."
            : "Usando reemplazos para:\n- " + string.Join("\n- ", report);
        Debug.Log("[Rescate] Escenas generadas: Menu (0), Nivel1_Jugueteria (1) y Nivel2_Calle (2), ya agregadas a Build Profiles.\n" + summary);
        EditorUtility.DisplayDialog("Listo",
            "Escenas Menu, Nivel1_Jugueteria y Nivel2_Calle generadas y agregadas a Build Profiles.\n\n" + summary +
            "\n\nDale Play desde la escena Menu para probar el recorrido completo.", "OK");
    }

    [MenuItem("Rescate/2. Build Windows (.exe)")]
    static void BuildWindows()
    {
        if (!BuildPipeline.IsBuildTargetSupported(BuildTargetGroup.Standalone, BuildTarget.StandaloneWindows64))
        {
            EditorUtility.DisplayDialog("Falta el módulo de Windows",
                "Instala 'Windows Build Support' desde Unity Hub > Installs > (tu versión) > Add modules.", "OK");
            return;
        }

        string[] scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
        if (scenes.Length == 0)
        {
            EditorUtility.DisplayDialog("Sin escenas", "Primero usa Rescate > 1. Generar escenas.", "OK");
            return;
        }

        PlayerSettings.productName = "Rescate del Conejo";
        PlayerSettings.fullScreenMode = FullScreenMode.Windowed;
        PlayerSettings.defaultScreenWidth = 1280;
        PlayerSettings.defaultScreenHeight = 720;
        PlayerSettings.resizableWindow = true;

        BuildReport result = BuildPipeline.BuildPlayer(new BuildPlayerOptions
        {
            scenes = scenes,
            locationPathName = ExePath,
            target = BuildTarget.StandaloneWindows64,
            options = BuildOptions.None,
        });

        if (result.summary.result == BuildResult.Succeeded)
        {
            EditorUtility.RevealInFinder(ExePath);
            EditorUtility.DisplayDialog("Build OK",
                $"Se generó {ExePath}\n\nAbre RescateDelConejo.exe y juega una partida completa (ganar Y perder) antes de entregar.\nEntrega la carpeta Builds/Windows COMPLETA, no solo el .exe.", "OK");
        }
        else
        {
            EditorUtility.DisplayDialog("Build falló",
                $"Resultado: {result.summary.result}\nErrores: {result.summary.totalErrors}\n\nRevisa la Console (rojo) y corrige el primer error de la lista.", "OK");
        }
    }

    // ------------------------------------------------------------------ escenas

    class Level
    {
        public string Path, Title, Next, WinMessage;
        public int Keys;
        public float StartInterval;
        public Sprite Background, Tile;
        public Color CameraColor;
    }

    static void BuildGameScene(GameAssets a, Level lv)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        Camera cam = CreateCamera(lv.CameraColor);
        CreateBackground(a, lv.Background, lv.Tile);

        // Conejo enjaulado: el objeto con animación en loop
        var rabbit = NewSprite("Conejo", a.Rabbit, a.RabbitTint, 1.2f, -1, a);
        rabbit.transform.position = new Vector3(cam.orthographicSize * 16f / 9f - 1f, 0f, 0f);
        rabbit.AddComponent<Animator>().runtimeAnimatorController = a.RabbitAnim;

        // Player
        GameObject player = NewSprite("Player", a.GirlIdle, a.GirlTint, 1.4f, 5, a);
        player.transform.position = new Vector3(-5.5f, LaneY[1], 0f);
        var rb = player.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 0f;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        var box = player.AddComponent<BoxCollider2D>();
        Sprite ps = player.GetComponent<SpriteRenderer>().sprite;
        box.size = ps.bounds.size * 0.7f;
        box.offset = ps.bounds.center;
        player.AddComponent<Animator>().runtimeAnimatorController = a.GirlAnim;

        var sfx = player.AddComponent<AudioSource>();
        sfx.playOnAwake = false;

        var stepsGo = new GameObject("Pasos");
        stepsGo.transform.SetParent(player.transform, false);
        var steps = stepsGo.AddComponent<AudioSource>();
        steps.clip = a.Steps;
        steps.loop = true;
        steps.playOnAwake = false;
        steps.volume = 0.4f;

        var pc = player.AddComponent<PlayerController>();
        Set(pc, "footsteps", steps);

        var th = player.AddComponent<TriggerHandler>();
        Set(th, "sfx", sfx);
        Set(th, "hitSfx", a.Hit);
        Set(th, "keySfx", a.KeyClip);
        Set(th, "heartSfx", a.HeartClip);
        Set(th, "giftSfx", a.GiftClip);
        Set(th, "body", player.GetComponent<SpriteRenderer>());
        Set(th, "confettiPrefab", a.ConfettiPrefab);

        // GameManager
        var gmGo = new GameObject("GameManager");
        var gm = gmGo.AddComponent<GameManager>();
        SetInt(gm, "keysToWin", lv.Keys);
        SetString(gm, "nextScene", lv.Next);

        // Spawner
        var spGo = new GameObject("Spawner");
        var sp = spGo.AddComponent<Spawner>();
        Set(sp, "player", pc);
        Set(sp, "monsterPrefab", a.MonsterPrefab);
        Set(sp, "keyPrefab", a.KeyPrefab);
        Set(sp, "heartPrefab", a.HeartPrefab);
        Set(sp, "giftPrefab", a.GiftPrefab);
        SetFloat(sp, "startInterval", lv.StartInterval);

        CreateMusic(a);

        // UI
        Transform canvas = CreateCanvas();
        CreateEventSystem();

        var heartIcon = NewImage("IconoCorazon", canvas, a.Heart, a.HeartTint);
        Place(heartIcon.rectTransform, new Vector2(0, 1), new Vector2(0.5f, 0.5f), new Vector2(72, -72), new Vector2(80, 80));
        var pulse = heartIcon.gameObject.AddComponent<HeartPulse>();

        var lives = NewText("TextoVidas", canvas, "Vidas: 3/3", 52, TextAlignmentOptions.Left);
        Place(lives.rectTransform, new Vector2(0, 1), new Vector2(0, 1), new Vector2(130, -32), new Vector2(500, 80));

        var keys = NewText("TextoLlaves", canvas, $"Llaves: 0/{lv.Keys}", 52, TextAlignmentOptions.Center);
        Place(keys.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -32), new Vector2(600, 80));

        var gifts = NewText("TextoRegalos", canvas, "Regalos: 0", 52, TextAlignmentOptions.Right);
        Place(gifts.rectTransform, new Vector2(1, 1), new Vector2(1, 1), new Vector2(-40, -32), new Vector2(500, 80));

        var tap = NewText("TocaParaEmpezar", canvas, $"{lv.Title}\n<size=48>Toca la pantalla o usa las flechas para empezar</size>", 72, TextAlignmentOptions.Center);
        Place(tap.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector2.zero, new Vector2(1700, 320));

        // Panel de fin
        var panel = NewImage("PanelFin", canvas, null, new Color(0f, 0f, 0f, 0.75f));
        Stretch(panel.rectTransform);
        var title = NewText("Titulo", panel.transform, lv.WinMessage, 88, TextAlignmentOptions.Center);
        Place(title.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), new Vector2(0, 150), new Vector2(1700, 260));
        var buttons = NewUI("Botones", panel.transform);
        Stretch(buttons);
        NewButton("BotonReintentar", buttons, "Reintentar", new Vector2(0, -60), gm.Restart);
        NewButton("BotonMenu", buttons, "Menú", new Vector2(0, -200), gm.GoToMenu);

        var hud = canvas.gameObject.AddComponent<HUDController>();
        Set(hud, "livesText", lives);
        Set(hud, "keysText", keys);
        Set(hud, "giftsText", gifts);
        Set(hud, "tapToStart", tap.gameObject);
        Set(hud, "endPanel", panel.gameObject);
        Set(hud, "endTitle", title);
        Set(hud, "heartPulse", pulse);
        Set(hud, "endButtons", buttons.gameObject);
        SetString(hud, "winMessage", lv.WinMessage);

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), lv.Path);
    }

    static void BuildMenuScene(GameAssets a)
    {
        EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);

        CreateCamera(new Color(0.45f, 0.25f, 0.2f));
        CreateBackground(a, a.BgToy, a.ToyTile);

        // Se agrandan con un padre: la animación de rebote controla la escala del hijo.
        var girl = NewSprite("Sprite", a.GirlIdle, a.GirlTint, 1.4f, 5, a);
        girl.AddComponent<Animator>().runtimeAnimatorController = a.GirlAnim;   // queda en Idle
        Wrap("Chica", girl, new Vector3(-3f, -1f, 0f), 1.6f);

        var rabbit = NewSprite("Sprite", a.Rabbit, a.RabbitTint, 1.2f, 5, a);
        rabbit.AddComponent<Animator>().runtimeAnimatorController = a.RabbitAnim;
        Wrap("Conejo", rabbit, new Vector3(3f, -1f, 0f), 1.6f);

        CreateMusic(a);

        Transform canvas = CreateCanvas();
        CreateEventSystem();

        var menu = canvas.gameObject.AddComponent<MenuController>();
        SetString(menu, "gameScene", Level1Name);

        var title = NewText("Titulo", canvas, "Rescate del Conejo", 120, TextAlignmentOptions.Center);
        Place(title.rectTransform, new Vector2(0.5f, 1), new Vector2(0.5f, 1), new Vector2(0, -80), new Vector2(1600, 200));

        NewButton("BotonJugar", canvas, "Jugar", new Vector2(0, 40), menu.Play);
        NewButton("BotonSalir", canvas, "Salir", new Vector2(0, -100), menu.Quit);

        EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene(), MenuScenePath);
    }

    // ------------------------------------------------------------------ piezas de escena

    static Camera CreateCamera(Color background)
    {
        var go = new GameObject("Main Camera");
        go.tag = "MainCamera";
        go.transform.position = new Vector3(0f, 0f, -10f);
        var cam = go.AddComponent<Camera>();
        cam.orthographic = true;
        cam.orthographicSize = CameraSize;
        cam.clearFlags = CameraClearFlags.SolidColor;
        cam.backgroundColor = background;
        go.AddComponent<AudioListener>();
        return cam;
    }

    static void CreateBackground(GameAssets a, Sprite background, Sprite tile)
    {
        if (background != null)
        {
            var bg = NewSprite("Fondo", background, Color.white, 1f, -10, a);
            Vector3 size = background.bounds.size;
            float scale = Mathf.Max(16f / size.x, CameraSize * 2f / size.y);
            bg.transform.localScale = Vector3.one * scale;
            return;
        }

        // Sin fondo propio: piso con baldosas repetidas (Draw Mode = Tiled) + líneas que marcan los carriles.
        var floor = new GameObject("Fondo");
        var sr = floor.AddComponent<SpriteRenderer>();
        sr.sprite = tile;
        sr.sharedMaterial = a.SpriteMaterial;
        sr.drawMode = SpriteDrawMode.Tiled;
        sr.size = new Vector2(16f, LaneY.Length * 2f);
        sr.sortingOrder = -10;

        for (int i = 0; i < LaneY.Length - 1; i++)
        {
            float y = (LaneY[i] + LaneY[i + 1]) / 2f;
            var line = NewSprite($"LineaCarril{i}", a.Square, new Color(0f, 0f, 0f, 0.25f), 1f, -9, a);
            line.transform.SetParent(floor.transform);
            line.transform.position = new Vector3(0f, y, 0f);
            line.transform.localScale = new Vector3(16f, 0.08f, 1f);
        }
    }

    static void CreateMusic(GameAssets a)
    {
        var go = new GameObject("Musica");
        var src = go.AddComponent<AudioSource>();
        src.clip = a.Music;
        src.loop = true;
        src.playOnAwake = true;
        src.volume = 0.5f;
    }

    static Transform CreateCanvas()
    {
        var go = new GameObject("Canvas", typeof(RectTransform), typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
        go.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
        var scaler = go.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        scaler.matchWidthOrHeight = 0.5f;
        return go.transform;
    }

    static void CreateEventSystem()
    {
        new GameObject("EventSystem", typeof(EventSystem), typeof(InputSystemUIInputModule));
    }

    static GameObject NewSprite(string name, Sprite sprite, Color tint, float height, int order, GameAssets a)
    {
        var go = new GameObject(name);
        var sr = go.AddComponent<SpriteRenderer>();
        sr.sprite = sprite;
        sr.color = tint;
        sr.sortingOrder = order;
        sr.sharedMaterial = a.SpriteMaterial;
        go.transform.localScale = Vector3.one * (height / sprite.bounds.size.y);
        return go;
    }

    static void Wrap(string name, GameObject child, Vector3 position, float scale)
    {
        var root = new GameObject(name);
        root.transform.position = position;
        root.transform.localScale = Vector3.one * scale;
        child.transform.SetParent(root.transform, false);
    }

    static GameObject MakeItemPrefab(string name, ItemKind kind, Sprite sprite, Color placeholderTint, float height, bool boxCollider,
        RuntimeAnimatorController anim, GameAssets a)
    {
        bool placeholder = sprite == a.Square || sprite == a.Circle;
        var go = NewSprite(name, sprite, placeholder ? placeholderTint : Color.white, height, 0, a);

        var rb = go.AddComponent<Rigidbody2D>();
        rb.bodyType = RigidbodyType2D.Kinematic;

        Collider2D col;
        if (boxCollider)
        {
            var b = go.AddComponent<BoxCollider2D>();
            b.size = sprite.bounds.size * 0.8f;
            b.offset = sprite.bounds.center;
            col = b;
        }
        else
        {
            var c = go.AddComponent<CircleCollider2D>();
            c.radius = Mathf.Max(sprite.bounds.extents.x, sprite.bounds.extents.y) * 0.8f;
            c.offset = sprite.bounds.center;
            col = c;
        }
        col.isTrigger = true;

        go.AddComponent<Mover>();
        var item = go.AddComponent<Item>();
        var so = new SerializedObject(item);
        so.FindProperty("kind").enumValueIndex = (int)kind;
        so.ApplyModifiedPropertiesWithoutUndo();

        if (anim != null)
        {
            go.AddComponent<Animator>().runtimeAnimatorController = anim;
        }

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabsDir}/{name}.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    static GameObject MakeConfettiPrefab()
    {
        var go = new GameObject("Confeti");
        var ps = go.AddComponent<ParticleSystem>();
        ps.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);

        var main = ps.main;
        main.duration = 1f;
        main.loop = false;
        main.playOnAwake = true;
        main.startLifetime = new ParticleSystem.MinMaxCurve(0.6f, 1.1f);
        main.startSpeed = new ParticleSystem.MinMaxCurve(3f, 6f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.08f, 0.16f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0f, Mathf.PI * 2f);
        main.gravityModifier = 1.2f;
        main.maxParticles = 100;
        main.stopAction = ParticleSystemStopAction.Destroy;   // se borra solo al terminar

        var gradient = new Gradient();
        gradient.SetKeys(
            new[]
            {
                new GradientColorKey(new Color(1f, 0.25f, 0.3f), 0f),
                new GradientColorKey(new Color(1f, 0.85f, 0.1f), 0.25f),
                new GradientColorKey(new Color(0.3f, 0.9f, 0.4f), 0.5f),
                new GradientColorKey(new Color(0.3f, 0.6f, 1f), 0.75f),
                new GradientColorKey(new Color(0.85f, 0.35f, 1f), 1f),
            },
            new[] { new GradientAlphaKey(1f, 0f), new GradientAlphaKey(1f, 1f) });
        var color = new ParticleSystem.MinMaxGradient(gradient);
        color.mode = ParticleSystemGradientMode.RandomColor;
        main.startColor = color;

        var emission = ps.emission;
        emission.rateOverTime = 0f;
        emission.SetBursts(new[] { new ParticleSystem.Burst(0f, 45) });

        var shape = ps.shape;
        shape.shapeType = ParticleSystemShapeType.Circle;
        shape.radius = 0.1f;

        var renderer = go.GetComponent<ParticleSystemRenderer>();
        renderer.sortingOrder = 10;
        renderer.sharedMaterial = ConfettiMaterial();

        GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, $"{PrefabsDir}/Confeti.prefab");
        Object.DestroyImmediate(go);
        return prefab;
    }

    // ------------------------------------------------------------------ UI

    static RectTransform NewUI(string name, Transform parent)
    {
        var go = new GameObject(name, typeof(RectTransform));
        go.transform.SetParent(parent, false);
        return go.GetComponent<RectTransform>();
    }

    static void Place(RectTransform rt, Vector2 anchor, Vector2 pivot, Vector2 pos, Vector2 size)
    {
        rt.anchorMin = anchor;
        rt.anchorMax = anchor;
        rt.pivot = pivot;
        rt.anchoredPosition = pos;
        rt.sizeDelta = size;
    }

    static void Stretch(RectTransform rt)
    {
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    static TextMeshProUGUI NewText(string name, Transform parent, string text, float size, TextAlignmentOptions align)
    {
        var t = NewUI(name, parent).gameObject.AddComponent<TextMeshProUGUI>();
        t.text = text;
        t.fontSize = size;
        t.alignment = align;
        t.color = Color.white;
        t.fontStyle = FontStyles.Bold;
        return t;
    }

    static Image NewImage(string name, Transform parent, Sprite sprite, Color color)
    {
        var img = NewUI(name, parent).gameObject.AddComponent<Image>();
        img.sprite = sprite;
        img.color = color;
        img.preserveAspect = sprite != null;
        return img;
    }

    static void NewButton(string name, Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        var img = NewImage(name, parent, null, new Color(1f, 0.82f, 0.3f));
        Place(img.rectTransform, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), pos, new Vector2(420, 110));
        var btn = img.gameObject.AddComponent<Button>();
        btn.targetGraphic = img;
        UnityEventTools.AddPersistentListener(btn.onClick, onClick);

        var text = NewText("Texto", img.transform, label, 56, TextAlignmentOptions.Center);
        text.color = new Color(0.2f, 0.12f, 0.05f);
        Stretch(text.rectTransform);
    }

    // ------------------------------------------------------------------ utilidades

    static void Set(Object target, string field, Object value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).objectReferenceValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetInt(Object target, string field, int value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).intValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetFloat(Object target, string field, float value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).floatValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void SetString(Object target, string field, string value)
    {
        var so = new SerializedObject(target);
        so.FindProperty(field).stringValue = value;
        so.ApplyModifiedPropertiesWithoutUndo();
    }

    static void PrepareFolders()
    {
        foreach (string dir in new[] { SpritesDir, AnimDir, PrefabsDir, AudioDir, ScenesDir })
        {
            Directory.CreateDirectory(dir);
        }
        foreach (string sub in ArtFolders)
        {
            Directory.CreateDirectory($"{ArtDir}/{sub}");
        }
        AssetDatabase.Refresh();
    }

    // Pixel art nítido: sin filtro y sin compresión en todo lo que esté en Assets/Art.
    static void FixPixelArtImport()
    {
        foreach (string guid in AssetDatabase.FindAssets("t:Texture2D", new[] { ArtDir }))
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            if (!(AssetImporter.GetAtPath(path) is TextureImporter ti)) continue;
            if (ti.textureType == TextureImporterType.Sprite &&
                ti.filterMode == FilterMode.Point &&
                ti.textureCompression == TextureImporterCompression.Uncompressed) continue;

            if (ti.textureType != TextureImporterType.Sprite)
            {
                ti.textureType = TextureImporterType.Sprite;
                ti.spriteImportMode = SpriteImportMode.Single;
            }
            ti.filterMode = FilterMode.Point;
            ti.textureCompression = TextureImporterCompression.Uncompressed;
            ti.SaveAndReimport();
        }
    }

    // ------------------------------------------------------------------ assets (tu arte o reemplazos)

    class GameAssets
    {
        public Sprite Square, Circle;
        public Material SpriteMaterial;

        public Sprite GirlIdle, Monster, Rabbit, Key, Heart, Gift;
        public Color GirlTint, RabbitTint, HeartTint;
        public RuntimeAnimatorController GirlAnim, RabbitAnim, MonsterAnim;
        public AudioClip Music, Hit, KeyClip, HeartClip, GiftClip, Steps;
        public Sprite BgToy, BgStreet, ToyTile, StreetTile;
        public GameObject MonsterPrefab, KeyPrefab, HeartPrefab, GiftPrefab, ConfettiPrefab;

        // Prefabs que instancia el Spawner (los mismos para los dos niveles).
        public void MakePrefabs()
        {
            MonsterPrefab = MakeItemPrefab("Monstruo", ItemKind.Monster, Monster, new Color(0.25f, 0.45f, 1f), 1.4f, true, MonsterAnim, this);
            KeyPrefab = MakeItemPrefab("Llave", ItemKind.Key, Key, new Color(1f, 0.85f, 0.1f), 0.8f, false, null, this);
            HeartPrefab = MakeItemPrefab("Corazon", ItemKind.Heart, Heart, new Color(1f, 0.2f, 0.3f), 0.8f, false, null, this);
            GiftPrefab = MakeItemPrefab("Regalo", ItemKind.Gift, Gift, new Color(0.7f, 0.3f, 1f), 0.8f, true, null, this);
            ConfettiPrefab = MakeConfettiPrefab();
        }

        public void Load()
        {
            Square = PlaceholderSprite("cuadrado", false);
            Circle = PlaceholderSprite("circulo", true);
            SpriteMaterial = LoadOrCreateSpriteMaterial();

            Sprite[] idle = LoadSprites("Chica/Idle");
            Sprite[] run = LoadSprites("Chica/Run");
            Sprite[] rabbit = LoadSprites("Conejo");
            Sprite[] monster = LoadSprites("Monstruo");

            GirlIdle = First(idle, Square, "Chica (Idle)", out bool girlPh);
            if (run.Length == 0) report.Add("Chica (Run): animación de rebote");
            GirlTint = girlPh ? new Color(1f, 0.5f, 0.75f) : Color.white;

            Rabbit = First(rabbit, Circle, "Conejo", out bool rabbitPh);
            RabbitTint = rabbitPh ? new Color(0.95f, 0.95f, 0.95f) : Color.white;

            Monster = First(monster, Square, "Monstruo", out _);
            Key = First(LoadSprites("Llave"), Circle, "Llave", out _);
            Heart = First(LoadSprites("Corazon"), Circle, "Corazón", out bool heartPh);
            HeartTint = heartPh ? new Color(1f, 0.2f, 0.3f) : Color.white;
            Gift = First(LoadSprites("Regalo"), Square, "Regalo", out _);
            Sprite[] toy = LoadSprites("Fondo/Jugueteria");
            BgToy = toy.Length > 0 ? toy[0] : null;
            if (BgToy == null) report.Add("Fondo juguetería: baldosas a cuadros");
            Sprite[] street = LoadSprites("Fondo/Calle");
            BgStreet = street.Length > 0 ? street[0] : null;
            if (BgStreet == null) report.Add("Fondo calle: adoquines");

            ToyTile = TileSprite("baldosa_jugueteria", (x, y) =>
                ((x / 8) + (y / 8)) % 2 == 0 ? new Color32(250, 225, 200, 255) : new Color32(240, 170, 185, 255));
            StreetTile = TileSprite("adoquines", Cobblestone);

            float girlScale = 1.4f / GirlIdle.bounds.size.y;
            float rabbitScale = 1.2f / Rabbit.bounds.size.y;

            AnimationClip idleClip = idle.Length >= 2
                ? SpriteClip("Chica_Idle", idle, 6f)
                : BounceClip("Chica_Idle", girlScale, 0.04f, 1.2f);
            AnimationClip runClip = run.Length >= 1
                ? SpriteClip("Chica_Run", run, 10f)
                : BounceClip("Chica_Run", girlScale, 0.15f, 0.3f);
            GirlAnim = IdleRunController(idleClip, runClip);

            AnimationClip rabbitClip = rabbit.Length >= 2
                ? SpriteClip("Conejo_Loop", rabbit, 6f)
                : BounceClip("Conejo_Loop", rabbitScale, 0.12f, 0.6f);
            RabbitAnim = LoopController("Conejo", rabbitClip);

            MonsterAnim = monster.Length >= 2 ? LoopController("Monstruo", SpriteClip("Monstruo_Loop", monster, 8f)) : null;

            Music = Clip("musica");
            Hit = Clip("golpe");
            KeyClip = Clip("llave");
            HeartClip = Clip("corazon");
            GiftClip = Clip("regalo");
            Steps = Clip("pasos");
        }

        static Sprite First(Sprite[] sprites, Sprite fallback, string label, out bool placeholder)
        {
            placeholder = sprites.Length == 0;
            if (placeholder) report.Add($"{label}: forma de color");
            return placeholder ? fallback : sprites[0];
        }

        static AudioClip Clip(string name)
        {
            string match = AssetDatabase.FindAssets($"{name} t:AudioClip", new[] { AudioDir })
                .Select(AssetDatabase.GUIDToAssetPath)
                .FirstOrDefault(p => Path.GetFileNameWithoutExtension(p).ToLowerInvariant() == name);
            if (match == null)
            {
                report.Add($"Audio '{name}': sin sonido (pon Assets/Audio/{name}.wav, .ogg o .mp3)");
                return null;
            }
            return AssetDatabase.LoadAssetAtPath<AudioClip>(match);
        }
    }

    static Sprite[] LoadSprites(string sub)
    {
        string folder = $"{ArtDir}/{sub}";
        if (!AssetDatabase.IsValidFolder(folder)) return new Sprite[0];

        return AssetDatabase.FindAssets("t:Sprite", new[] { folder })
            .Distinct()
            .Select(AssetDatabase.GUIDToAssetPath)
            .Where(p => Path.GetDirectoryName(p).Replace('\\', '/') == folder)   // no mezclar subcarpetas
            .SelectMany(p => AssetDatabase.LoadAllAssetsAtPath(p).OfType<Sprite>())
            .Distinct()
            .OrderBy(s => s.name.Length)      // "run_2" antes que "run_10"
            .ThenBy(s => s.name)
            .ToArray();
    }

    static Sprite PlaceholderSprite(string name, bool circle)
    {
        string path = $"{SpritesDir}/{name}.png";
        if (!File.Exists(path))
        {
            const int n = 16;
            var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
            var px = new Color32[n * n];
            for (int y = 0; y < n; y++)
            for (int x = 0; x < n; x++)
            {
                float dx = x - (n - 1) / 2f, dy = y - (n - 1) / 2f;
                bool inside = !circle || dx * dx + dy * dy <= (n / 2f) * (n / 2f);
                px[y * n + x] = inside ? new Color32(255, 255, 255, 255) : new Color32(0, 0, 0, 0);
            }
            tex.SetPixels32(px);
            File.WriteAllBytes(path, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);
            AssetDatabase.ImportAsset(path);
        }

        var ti = (TextureImporter)AssetImporter.GetAtPath(path);
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = 16;
        ti.filterMode = FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // Baldosa de 16x16 px que se repite (para Draw Mode = Tiled).
    static Sprite TileSprite(string name, System.Func<int, int, Color32> pixel)
    {
        string path = $"{SpritesDir}/{name}.png";
        const int n = 16;
        var tex = new Texture2D(n, n, TextureFormat.RGBA32, false);
        var px = new Color32[n * n];
        for (int y = 0; y < n; y++)
        for (int x = 0; x < n; x++)
        {
            px[y * n + x] = pixel(x, y);
        }
        tex.SetPixels32(px);
        File.WriteAllBytes(path, tex.EncodeToPNG());
        Object.DestroyImmediate(tex);
        AssetDatabase.ImportAsset(path);

        var ti = (TextureImporter)AssetImporter.GetAtPath(path);
        ti.textureType = TextureImporterType.Sprite;
        ti.spriteImportMode = SpriteImportMode.Single;
        ti.spritePixelsPerUnit = 16;
        ti.filterMode = FilterMode.Point;
        ti.textureCompression = TextureImporterCompression.Uncompressed;
        ti.wrapMode = TextureWrapMode.Repeat;
        var settings = new TextureImporterSettings();
        ti.ReadTextureSettings(settings);
        settings.spriteMeshType = SpriteMeshType.FullRect;   // necesario para Tiled
        ti.SetTextureSettings(settings);
        ti.SaveAndReimport();
        return AssetDatabase.LoadAssetAtPath<Sprite>(path);
    }

    // Adoquines al atardecer: piedras oscuras de 8x8, junta casi negra y un brillo cálido en el borde.
    static Color32 Cobblestone(int x, int y)
    {
        int row = y / 8;
        int lx = (x + (row % 2) * 4) % 8;
        int ly = y % 8;
        bool joint = lx == 0 || ly == 0;
        bool corner = (lx == 1 || lx == 7) && (ly == 1 || ly == 7);
        if (joint || corner) return new Color32(28, 27, 36, 255);

        int stone = ((x + (row % 2) * 4) / 8) * 7 + row * 13;
        int shade = 62 + (stone * 37) % 26;
        bool warm = lx <= 2 && ly >= 5;    // reflejo del sol bajo, arriba a la izquierda de cada piedra
        return warm ? new Color32((byte)(shade + 28), (byte)(shade + 13), (byte)(shade + 10), 255)
                    : new Color32((byte)(shade + 6), (byte)shade, (byte)(shade + 10), 255);
    }

    // Material sin luces: los sprites se ven siempre, aunque la escena no tenga Light 2D.
    static Material LoadOrCreateSpriteMaterial()
    {
        string path = $"{GenDir}/SpriteSinLuz.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        Shader shader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        mat = new Material(shader);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    static Material ConfettiMaterial()
    {
        string path = $"{GenDir}/Confeti.mat";
        var mat = AssetDatabase.LoadAssetAtPath<Material>(path);
        if (mat != null) return mat;

        Shader shader = Shader.Find("Universal Render Pipeline/Particles/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        mat = new Material(shader);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    // ------------------------------------------------------------------ animaciones

    static AnimationClip SpriteClip(string name, Sprite[] frames, float fps)
    {
        var clip = new AnimationClip { frameRate = fps };
        var binding = EditorCurveBinding.PPtrCurve("", typeof(SpriteRenderer), "m_Sprite");
        var keys = new ObjectReferenceKeyframe[frames.Length + 1];
        for (int i = 0; i < frames.Length; i++)
        {
            keys[i] = new ObjectReferenceKeyframe { time = i / fps, value = frames[i] };
        }
        // Clave extra al final para que el último cuadro dure lo mismo que los demás.
        keys[frames.Length] = new ObjectReferenceKeyframe { time = frames.Length / fps, value = frames[frames.Length - 1] };
        AnimationUtility.SetObjectReferenceCurve(clip, binding, keys);
        return SaveLoopClip(clip, name);
    }

    // Reemplazo cuando no hay cuadros: el objeto "respira" (se estira y aplasta).
    static AnimationClip BounceClip(string name, float baseScale, float amount, float period)
    {
        var clip = new AnimationClip { frameRate = 30f };
        float half = period / 2f;
        clip.SetCurve("", typeof(Transform), "m_LocalScale.x",
            new AnimationCurve(new Keyframe(0, baseScale), new Keyframe(half, baseScale * (1f + amount)), new Keyframe(period, baseScale)));
        clip.SetCurve("", typeof(Transform), "m_LocalScale.y",
            new AnimationCurve(new Keyframe(0, baseScale), new Keyframe(half, baseScale * (1f - amount)), new Keyframe(period, baseScale)));
        clip.SetCurve("", typeof(Transform), "m_LocalScale.z",
            AnimationCurve.Constant(0, period, baseScale));
        return SaveLoopClip(clip, name);
    }

    static AnimationClip SaveLoopClip(AnimationClip clip, string name)
    {
        AnimationClipSettings settings = AnimationUtility.GetAnimationClipSettings(clip);
        settings.loopTime = true;
        AnimationUtility.SetAnimationClipSettings(clip, settings);

        string path = $"{AnimDir}/{name}.anim";
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(clip, path);
        return clip;
    }

    static AnimatorController IdleRunController(AnimationClip idle, AnimationClip run)
    {
        string path = $"{AnimDir}/Chica.controller";
        AssetDatabase.DeleteAsset(path);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        controller.AddParameter("IsRunning", AnimatorControllerParameterType.Bool);

        AnimatorStateMachine sm = controller.layers[0].stateMachine;
        AnimatorState idleState = sm.AddState("Idle");
        idleState.motion = idle;
        AnimatorState runState = sm.AddState("Run");
        runState.motion = run;
        sm.defaultState = idleState;

        AnimatorStateTransition toRun = idleState.AddTransition(runState);
        toRun.hasExitTime = false;
        toRun.duration = 0f;
        toRun.AddCondition(AnimatorConditionMode.If, 0f, "IsRunning");

        AnimatorStateTransition toIdle = runState.AddTransition(idleState);
        toIdle.hasExitTime = false;
        toIdle.duration = 0f;
        toIdle.AddCondition(AnimatorConditionMode.IfNot, 0f, "IsRunning");

        return controller;
    }

    static AnimatorController LoopController(string name, AnimationClip clip)
    {
        string path = $"{AnimDir}/{name}.controller";
        AssetDatabase.DeleteAsset(path);
        var controller = AnimatorController.CreateAnimatorControllerAtPath(path);
        AnimatorState state = controller.layers[0].stateMachine.AddState("Loop");
        state.motion = clip;
        controller.layers[0].stateMachine.defaultState = state;
        return controller;
    }
}
