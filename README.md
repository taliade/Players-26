# Players-26 — Rescate del Conejo (Unity 2D)

Scripts y guía para armar el MVP de la Pre-Entrega. El diseño está en [`GDD.md`](GDD.md).
**Si es tu primera vez con Unity, empieza por [`GUIA.md`](GUIA.md)**: paso a paso, glosario y modos de juego.
Para probar la mecánica sin Unity, abre [`demo/index.html`](demo/index.html) en el navegador (las mismas reglas, arte y sonido de prueba). La web para publicar está en [`web/rescatando-al-conejo/`](web/rescatando-al-conejo/LEEME.md).

> El proyecto Unity se crea en tu PC. Este repo trae los scripts: copia `Assets/Scripts` dentro de la carpeta `Assets` de tu proyecto.

## 0. Antes de todo (primera hora)
1. Unity Hub → Unity 6 LTS → plantilla **Universal 2D**.
2. Copia las carpetas `Assets/Scripts` y `Assets/Editor` de este repo dentro de `Assets` de tu proyecto.
   **Ojo:** si modificaste los scripts a mano, compáralos antes de pisarlos.
3. *Window → TextMeshPro → Import TMP Essential Resources*.
4. Menú **Rescate → 1. Generar escenas**. Crea `Menu`, `Nivel1_Jugueteria` (3 llaves) y `Nivel2_Calle` (5 llaves, más rápido) con todo conectado y las agrega a Build Profiles en ese orden. **Reemplaza la lista de escenas que tuvieras antes.**
5. Abre la escena `Menu` y dale Play.
6. Menú **Rescate → 2. Build Windows (.exe)** → `Builds/Windows/RescateDelConejo.exe`. Entrega la carpeta **completa**.
7. Si quieres Android: instala *Android Build Support* desde Unity Hub. **iOS no es posible sin Mac.**

### Tu arte y tu audio (opcional, se toma solo al regenerar)
| Carpeta / archivo | Qué poner |
|---|---|
| `Assets/Art/Chica/Idle` y `Assets/Art/Chica/Run` | Cuadros de animación (`idle_0.png`, `idle_1.png`… o una hoja recortada) |
| `Assets/Art/Monstruo`, `Assets/Art/Conejo` | Cuadros (2 o más = animación en loop) |
| `Assets/Art/Llave`, `Corazon`, `Regalo` | Un sprite cada una |
| `Assets/Art/Fondo/Jugueteria`, `Assets/Art/Fondo/Calle` | Un fondo cada una (si están vacías: baldosas a cuadros y adoquines generados) |
| `Assets/Audio/musica`, `golpe`, `llave`, `corazon`, `regalo`, `pasos` | `.wav`, `.ogg` o `.mp3` con **ese nombre exacto** |

Si falta algo, el generador usa formas de colores y animaciones de rebote, y te dice qué reemplazó.

## 1. Matriz de componentes

| GameObject | Componentes | Configuración clave |
|---|---|---|
| **Player** (chica) | SpriteRenderer, Rigidbody2D, BoxCollider2D, Animator, AudioSource, `PlayerController`, `TriggerHandler` | Rigidbody2D **Dynamic**, Gravity Scale 0, Freeze Rotation Z. AudioSource con Play On Awake **apagado** (es para SFX). |
| **Prefab Monstruo** | SpriteRenderer, Rigidbody2D, BoxCollider2D, `Mover`, `Item` | Collider **Is Trigger**. `Item.Kind` = Monster. (Rigidbody2D lo pasa a Kinematic el script.) |
| **Prefab Llave / Corazón / Regalo** | SpriteRenderer, Rigidbody2D, CircleCollider2D, `Mover`, `Item` | Collider **Is Trigger**. `Item.Kind` = Key / Heart / Gift. |
| **Conejo enjaulado** (loop) | SpriteRenderer, Animator | Clip de 2–4 frames con **Loop Time** activado. Sin collider: es decorado. |
| **Spawner** | `Spawner` | Arrastra Player y los 4 prefabs. Posición indiferente. |
| **GameManager** | `GameManager` | Max Lives 3, Keys To Win 5. |
| **Music** | AudioSource | Clip de música, **Play On Awake** y **Loop** activados. |
| **Canvas** | Canvas, Canvas Scaler, `HUDController` | Scale With Screen Size, 1920×1080. Textos TMP: Vidas, Llaves, Regalos, "Toca para empezar". Panel Fin con título + botón **Reintentar** → OnClick → `GameManager.Restart`. |
| **EventSystem** | EventSystem, **InputSystemUIInputModule** | Si Unity te propone reemplazar el StandaloneInputModule, acepta. Si no, el botón no responde. |
| **Main Camera** | Camera | Orthographic, Size 3–4. Fondo de pasto con 3 carriles en Y = 2, 0, −2. |

## 2. Animator del Player
- Dos clips: `Idle` y `Run`.
- Parámetro **Bool `IsRunning`** (con ese nombre exacto: el código lo busca así).
- Transiciones: Idle → Run si `IsRunning == true`; Run → Idle si `false`. **Has Exit Time apagado** y Transition Duration en 0 (para pixel art).

## 3. Pixel art (importación)
En cada sprite: Filter Mode **Point (no filter)**, Compression **None** y el mismo **Pixels Per Unit** para todos.
Si no tienes arte: usa packs gratuitos (por ejemplo de kenney.nl, que son CC0, o de itch.io revisando la licencia) y anota los créditos en el GDD.

## 4. Scripts y qué requisito cubren

| Script | Responsabilidad | Requisito |
|---|---|---|
| `PlayerController` | Lee el toque/clic, cambia de carril y maneja Idle/Run | Inputs, Variables, Condicionales, Funciones, Animación |
| `Spawner` | Corrutina que instancia monstruos y objetos cada vez más rápido | **Instantiate**, intervalos de tiempo |
| `TriggerHandler` | `OnTriggerEnter2D`: decide qué tocaste, suena el SFX y avisa | **Colisiones**, Condicionales (`switch`), SFX |
| `Mover` | Mueve hacia la izquierda y destruye al salir de cámara | Variables, Funciones |
| `Item` | Dice qué es cada prefab (Monstruo/Llave/Corazón/Regalo) | — (datos) |
| `GameManager` | Vidas, llaves, victoria/derrota, reinicio con SceneManager | Condicionales, Funciones, Reset |
| `HUDController` | Muestra vidas/llaves y el panel final | UI |
| `HeartPulse` | El corazón del HUD late al recuperar vida | Feedback |
| `MenuController` | Botones Jugar / Salir | Escenas |
| `Editor/SceneBuilder` | Genera escenas, prefabs, animaciones y el .exe | Build |

## 5. Checklist antes del build
- [ ] ¿El Player responde al toque/clic y cambia de carril correctamente?
- [ ] ¿El Spawner usa `Instantiate` y aparecen monstruos, llaves y corazones?
- [ ] ¿Al chocar pasa algo visible? (vida baja / llave sube / el objeto desaparece)
- [ ] ¿La chica pasa de Idle a Run al empezar, y el conejo se anima en loop sin parar?
- [ ] ¿Suenan la música de fondo y los SFX de golpe y recolección?
- [ ] ¿La consola no tiene errores, la escena está en la lista de Build Profiles y el ejecutable abre?
