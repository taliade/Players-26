# Guía para principiantes: *Rescate del Conejo* en Unity 2D

Esta guía te lleva de cero a un juego que se puede abrir como ejecutable (.exe). Tiene dos escenarios: **la juguetería** y **la calle de adoquines**.

Está escrita para alguien que nunca hizo un videojuego. Si una palabra no la entiendes, búscala en el **Glosario (Parte 1)**.

**Cómo usarla:**
- Haz los pasos en orden. Cada uno termina con un **✅ Comprueba**: no pases al siguiente hasta que funcione.
- Si te trabas, mira la **Parte 6: Errores comunes**.
- Si estás corto de tiempo, hay un atajo: el menú **Rescate → 1. Generar escenas** arma todo esto solo (ver el `README.md`). Igual te conviene leer la guía, porque en la evaluación te van a preguntar cómo funciona.

---

## Índice
1. [Glosario](#parte-1--glosario)
2. [El diseño del juego](#parte-2--el-diseño-del-juego)
3. [Paso a paso en Unity](#parte-3--paso-a-paso-en-unity)
4. [Segundo escenario: la calle](#parte-4--segundo-escenario-la-calle)
5. [Modos de juego: 1 jugador, contra la PC y multijugador](#parte-5--modos-de-juego)
6. [Errores comunes](#parte-6--errores-comunes)
7. [Checklist de la Pre-Entrega](#parte-7--checklist-de-la-pre-entrega)

---

## Parte 1 — Glosario

### 1.1 Conceptos básicos de diseño

| Término | Qué significa | En nuestro juego |
|---|---|---|
| **GDD** (*Game Design Document*) | El documento que describe el juego: de qué trata, para quién es, cómo se juega. | `GDD.md` y la plantilla del curso. |
| **MVP** (*Producto Mínimo Viable*) | La versión más chica del juego que ya se puede jugar y probar. | 2 niveles, 1 enemigo, 3 objetos. |
| **Alcance** (*scope*) | Cuánto vas a hacer. El error número uno de un principiante es poner demasiado. | Todo lo de la Parte 5 queda **fuera** del alcance de la Pre-Entrega. |
| **Mecánica** | Una regla con la que el jugador interactúa. | Cambiar de carril. |
| **Mecánica principal** (*core mechanic*) | La acción que el jugador hace todo el tiempo. | Cambiar de carril para esquivar o agarrar cosas. |
| **Bucle de juego** (*core loop*) | El ciclo que se repite: acción → respuesta del juego → consecuencia. | Cambio de carril → sonido y efecto → +1 llave o −1 vida. |
| **MDA** | Mecánicas (las reglas) → Dinámicas (lo que pasa al jugar) → Estética (lo que siente el jugador). | Carriles + monstruos → esquivar a último momento → tensión y alivio. |
| **Condición de victoria / derrota** | Qué hace falta para ganar o para perder. | Ganar: juntar las llaves. Perder: quedarse con 0 vidas. |
| **Riesgo / recompensa** | Una decisión en la que lo valioso está en un lugar peligroso. | Una llave en el carril donde viene un monstruo. |
| **Curva de dificultad** | Cómo el juego se pone más difícil con el tiempo. | El Spawner acorta el intervalo entre apariciones; el nivel 2 arranca más rápido. |
| **Feedback** | Cómo el juego te avisa que algo pasó. | El destello rojo, los sonidos, el confeti, el corazón que late. |
| ***Game feel* / *juice*** | Los detalles que hacen que el juego "se sienta bien" al jugarlo. | El confeti y el latido. **Se agregan al final, nunca primero.** |
| ***Greybox*** | Armar el juego con cuadrados y círculos antes de poner el arte. | Los Pasos 5 a 12 de esta guía. |
| **Prototipo** | Una versión rápida para probar si una idea funciona. | La demo web. |
| ***Playtest*** | Hacer que otra persona juegue mientras tú miras **en silencio** y anotas. | Hazlo antes de entregar. |
| **Build** | El juego empaquetado para abrirlo sin Unity. | `RescateDelConejo.exe`. |

### 1.2 Géneros (tipos de juego)

| Género | Qué es | Ejemplos |
|---|---|---|
| **Arcade** | Partidas cortas, reglas simples, puntaje. | *Pac-Man*, *Space Invaders* |
| ***Endless runner*** | El personaje corre solo y tú esquivas. | *Subway Surfers*, *Temple Run* |
| ***Lane runner*** (de carriles) | Un *runner* donde te mueves entre carriles fijos. | *Subway Surfers* (3 carriles) |
| ***Tower defense*** | Defiendes un lugar de enemigos que avanzan por caminos. | *Plants vs Zombies* |
| **Plataformas** | Saltar entre plataformas. | *Mario*, *Celeste* |
| **Puzzle** | Resolver problemas o acertijos. | *Tetris*, *Candy Crush* |
| ***Shoot 'em up*** | Naves que disparan a oleadas de enemigos. | *Galaga* |
| **RPG** | Tu personaje sube de nivel y hay una historia larga. | *Pokémon* |
| **Aventura** | Explorar y avanzar en una historia. | *Zelda* |
| ***Roguelike*** | Niveles aleatorios; al morir empiezas de nuevo. | *Hades*, *Dead Cells* |
| ***Party game*** | Para jugar varias personas juntas. | *Mario Party*, *Overcooked* |
| **Casual / hipercasual** | Se entiende en segundos y se juega con un dedo. | *Flappy Bird*, *Crossy Road* |

**Nuestro juego es un *lane runner* arcade.** De *Plants vs Zombies* toma los carriles y los enemigos que avanzan de derecha a izquierda. De *Subway Surfers* toma cambiar de carril para esquivar y juntar cosas.

### 1.3 Mecánicas comunes

| Mecánica | Qué es | ¿Está en el juego? |
|---|---|---|
| Movimiento libre | Moverse en cualquier dirección. | No: el movimiento es por carriles, que es más simple. |
| **Movimiento por carriles** | Solo hay posiciones fijas. | **Sí** |
| **Esquivar** | Evitar algo peligroso. | **Sí** (monstruos) |
| **Recolectar** | Tocar un objeto para agarrarlo. | **Sí** (llaves, corazones, regalos) |
| **Vidas** | Cuántos errores puedes cometer. | **Sí** (3) |
| **Curación** | Recuperar vida. | **Sí** (corazón, máximo 3) |
| **Spawn aleatorio** | Los objetos aparecen al azar según probabilidades. | **Sí** (Spawner) |
| **Dificultad progresiva** | El juego acelera. | **Sí** |
| **Niveles** | El juego avanza por etapas. | **Sí** (juguetería → calle) |
| *Power-up* | Una mejora temporal (escudo, imán…). | No. Es una idea para después. |
| *I-frames* | Invulnerabilidad unos segundos después de un golpe. | No: el monstruo desaparece al tocarte, y eso alcanza. |
| Puntaje / récord | Un número que compite con tu mejor partida. | Parcial: los regalos suman, pero el récord no se guarda. |
| IA (inteligencia artificial) | Un personaje que maneja la computadora. | Lo explica la Parte 5 (fuera de la Pre-Entrega). |
| Multijugador local | Varias personas en la misma computadora. | Lo explica la Parte 5 (fuera de la Pre-Entrega). |

### 1.4 Tipos de arte

| Estilo | Qué es | Ventaja / desventaja para ti |
|---|---|---|
| ***Pixel art*** | Dibujos con píxeles grandes y visibles, estilo retro. | ✅ Sprites chicos y animaciones de 2 a 4 cuadros. Es el que elegiste. |
| ***Flat* / vectorial** | Formas planas con colores lisos. | ✅ Fácil de hacer en Canva o Figma. |
| **Dibujado a mano** | Ilustración o acuarela. | ⚠️ Lindo, pero animarlo lleva mucho tiempo. |
| ***Low poly* 3D** | Modelos 3D con pocas caras. | ❌ Esto es 2D: no aplica. |
| ***Voxel*** | Pixel art en 3D, hecho con cubitos (como *Minecraft*). | ❌ No aplica. |
| **Isométrico** | Vista en diagonal que parece 3D. | ⚠️ Complica los carriles. |
| ***Parallax*** | Capas de fondo que se mueven a distinta velocidad y dan profundidad. | Idea para después. |

**Palabras de *pixel art* y de Unity 2D:**

| Término | Qué significa |
|---|---|
| **Sprite** | Una imagen 2D dentro del juego. |
| ***Spritesheet*** | Una sola imagen con todos los cuadros de una animación. Se recorta con el *Sprite Editor*. |
| **Frame** (cuadro) | Una imagen de una animación. Run puede tener 4 frames. |
| **Tile / tileset** | Una baldosa que se repite para armar el piso (los adoquines, por ejemplo). |
| **Paleta** | Los colores que usa el juego. Pocos colores = estilo más coherente. |
| **PPU** (*Pixels Per Unit*) | Cuántos píxeles del dibujo entran en 1 unidad de Unity. **Usa el mismo PPU en todo tu arte.** |
| **Filtro Point** | Hace que el pixel art se vea nítido, sin borroneado. **Obligatorio para *pixel art*.** |
| **Sorting Order** | Qué se dibuja adelante y qué atrás. Un número más alto se dibuja más adelante. |

### 1.5 Palabras de Unity

| Término | Qué significa |
|---|---|
| **Escena** (*Scene*) | Un "nivel" o pantalla del juego: Menu, Nivel1_Jugueteria, Nivel2_Calle. |
| **GameObject** | Cualquier cosa que está en la escena: la chica, la cámara, un texto. |
| **Componente** | Una pieza que le da comportamiento a un GameObject: SpriteRenderer, Rigidbody2D, un script. |
| **Transform** | Posición, rotación y escala. Todo GameObject tiene uno. |
| **Hierarchy** | La lista de GameObjects de la escena abierta. |
| **Inspector** | El panel donde ves y cambias los componentes del objeto seleccionado. |
| **Project** | Tus archivos: sprites, scripts, sonidos, escenas. |
| **Prefab** | Un GameObject guardado como "molde" para crear copias (el monstruo, por ejemplo). |
| **Instantiate** | La función de C# que crea una copia de un prefab mientras el juego corre. |
| **Rigidbody2D** | Hace que un objeto participe de la física. *Dynamic* = lo mueve la física; *Kinematic* = lo mueve tu código. |
| **Collider2D** | La forma invisible que detecta choques (*hitbox*). |
| **Trigger** | Un collider que **detecta** el contacto pero **no empuja**. Llama a `OnTriggerEnter2D`. |
| **Animator / Animation Clip** | El Clip es una animación (Idle). El Animator decide qué clip se reproduce. |
| **Parámetro del Animator** | Una variable (`IsRunning`) que el código cambia para pasar de un clip a otro. |
| **AudioSource / AudioClip** | El AudioSource es el "parlante" y el AudioClip es el archivo de sonido. |
| **Canvas** | El "lienzo" de la interfaz (textos, botones). |
| **TextMeshPro (TMP)** | El sistema de textos de Unity. |
| **HUD** | La información en pantalla mientras juegas: vidas, llaves. |
| **Input System** | El sistema de Unity para leer teclado, mouse, touch y joysticks. |
| **Build Profiles** | Donde eliges la plataforma (Windows) y la lista de escenas del build. |
| **Console** | El panel donde aparecen los mensajes y los **errores (en rojo)**. |

---

## Parte 2 — El diseño del juego

**En una frase:** una chica esquiva en tres carriles a los monstruos azules que avanzan como los zombies de *Plants vs Zombies*, y junta llaves para rescatar a su conejo.

**Historia, en 3 líneas:**
1. **Nivel 1, la juguetería:** el monstruo azul encierra al conejo y cierra la puerta. Juntas **3 llaves** y abres la puerta.
2. El monstruo escapa a la calle con el conejo.
3. **Nivel 2, la calle de adoquines al atardecer:** oscura, con faroles que se encienden, y más rápida. Juntas **5 llaves** y liberas al conejo.

| | Nivel 1: Juguetería | Nivel 2: Calle |
|---|---|---|
| Fondo | Piso de baldosas | Adoquines |
| Llaves para ganar | 3 | 5 |
| Intervalo inicial del Spawner | 1,5 s | 1,1 s (más difícil) |
| Al ganar | Pasa solo al nivel 2 a los 2 segundos | Pantalla "¡Liberaste al conejo!" |
| Al perder | Reintentar o volver al menú | Reintentar o volver al menú |

**Decisión de diseño:** cada nivel empieza con 3 vidas. Arrastrar las vidas de un nivel al otro es más "real", pero agrega estado que sobrevive entre escenas, y eso es una fuente típica de errores. Para el MVP no conviene.

**Personajes:** Chica (jugadora), Monstruo azul (enemigo), Conejo (objetivo, con animación en loop).

---

## Parte 3 — Paso a paso en Unity

> Calcula unas **8 a 12 horas** si es tu primera vez. Los pasos están en orden de importancia: si te quedas sin tiempo, lo que falta es lo menos importante.

### Paso 1 — Crear el proyecto
1. Abre **Unity Hub → New project**.
2. Elige **Unity 6 LTS** y la plantilla **Universal 2D**.
3. Nombre: `RescateDelConejo`. Crear.
4. En **Unity Hub → Installs → tu versión → Add modules**, verifica que esté **Windows Build Support**.
5. Haz ya un build de prueba: **File → Build Profiles → Windows → Build**, en una carpeta `Builds/Test`.

✅ **Comprueba:** se generó un .exe y abre una pantalla azul. Si esto falla, mejor descubrirlo ahora que a una hora de la entrega.

### Paso 2 — Carpetas
En la ventana **Project**, dentro de `Assets`, crea: `Scripts`, `Art`, `Audio`, `Prefabs`, `Animations`, `Scenes`.

Copia los scripts de este repo (`Assets/Scripts/*.cs`) a tu carpeta `Scripts`. Espera a que Unity termine de compilar (el ícono que gira abajo a la derecha).

✅ **Comprueba:** la Console no muestra nada en rojo.

### Paso 3 — Importar el arte (*pixel art*)
1. Arrastra tus imágenes a `Assets/Art`.
2. Selecciónalas todas y, en el Inspector, configura:
   - **Texture Type:** Sprite (2D and UI)
   - **Pixels Per Unit:** el mismo para todas (por ejemplo, 16 o 32)
   - **Filter Mode:** **Point (no filter)**
   - **Compression:** **None**
3. Presiona **Apply**.
4. Si una imagen es una *spritesheet* (varios cuadros juntos): **Sprite Mode = Multiple → Sprite Editor → Slice → Grid By Cell Size** → Apply.

¿No tienes arte? Usa packs gratuitos de [kenney.nl](https://kenney.nl) (CC0) o de itch.io (revisa la licencia de cada uno). Mientras tanto, trabaja con cuadrados: *Assets → Create → 2D → Sprites → Square*.

✅ **Comprueba:** al hacer zoom, los píxeles se ven con bordes duros y no borrosos.

### Paso 4 — Escena del Nivel 1
1. **File → New Scene → Basic 2D (URP)**.
2. Guárdala como `Assets/Scenes/Nivel1_Jugueteria`.
3. Selecciona **Main Camera** y configura:
   - Projection: **Orthographic**
   - Size: **4**
   - Position: (0, 0, −10)

### Paso 5 — Fondo y carriles
1. Arrastra el sprite del piso de la juguetería a la escena. Renómbralo `Fondo`.
2. En su SpriteRenderer pon **Order in Layer: −10**.
3. Si es una baldosa que se repite:
   - En el sprite (no en el objeto): **Mesh Type = Full Rect** y **Wrap Mode = Repeat** → Apply.
   - En el SpriteRenderer del objeto: **Draw Mode = Tiled** y **Size = (16, 6)**.
4. Los carriles quedan en **Y = 2, 0 y −2**. Para que se vean, puedes poner 2 líneas finas en Y = 1 y Y = −1 (cuadrados con Scale (16, 0.08)).

✅ **Comprueba:** en la pestaña **Game**, con relación 16:9, el piso llena toda la pantalla.

### Paso 6 — La chica (Player)
1. Arrastra el primer cuadro Idle de la chica a la escena. Renómbrala `Player`.
2. Position: (−5.5, 0, 0). Order in Layer: 5.
3. **Add Component** y agrega, en este orden:

| Componente | Configuración |
|---|---|
| **Rigidbody2D** | Body Type: **Dynamic** · Gravity Scale: **0** · Constraints → Freeze Rotation Z ✔ · Interpolate: Interpolate |
| **BoxCollider2D** | Click en *Edit Collider* y achícalo un poco respecto del dibujo (es más justo para el jugador) |
| **Animator** | Lo configuras en el Paso 7 |
| **AudioSource** | **Play On Awake ✘** (es para los efectos) |
| **PlayerController** (script) | Lane Y: 2, 0, −2 · Switch Speed: 20 |
| **TriggerHandler** (script) | Arrastra el AudioSource al campo *Sfx* y el SpriteRenderer al campo *Body* |

✅ **Comprueba:** dale **Play**. Haz clic arriba o abajo, o usa ↑ ↓ / W S: la chica cambia de carril. La Console va a decir que falta el GameManager; eso se arregla en el Paso 10.

> **¿Por qué Dynamic y no Kinematic?** Para que `OnTriggerEnter2D` se dispare, al menos uno de los dos objetos tiene que tener un Rigidbody2D. Con la chica en Dynamic y los objetos en Kinematic, el choque se detecta siempre.

### Paso 7 — Animaciones Idle y Run
1. Abre **Window → Animation → Animation**, con el Player seleccionado.
2. **Create** → guarda `Chica_Idle.anim` en `Animations`.
3. Arrastra los cuadros de Idle a la línea de tiempo. Sample: 6.
4. En el menú del clip: **Create New Clip** → `Chica_Run.anim`. Arrastra los cuadros de Run. Sample: 10.
5. Abre **Window → Animation → Animator**:
   - En **Parameters**, agrega un **Bool** llamado `IsRunning`, con ese nombre exacto.
   - Clic derecho en Idle → **Make Transition** → Run. En la flecha: **Has Exit Time ✘**, **Transition Duration = 0**, Condition: `IsRunning` = **true**.
   - Haz la transición de Run → Idle igual, con `IsRunning` = **false**.
6. Selecciona cada clip en Project y verifica que tenga **Loop Time ✔**.

✅ **Comprueba:** en Play, la chica respira en Idle. Al primer toque pasa a Run.

### Paso 8 — Prefabs (monstruo, llave, corazón, regalo)
Para **cada uno**:
1. Arrastra el sprite a la escena. Position: (0, 0, 0).
2. Agrega estos componentes:
   - **Rigidbody2D** → Body Type: **Kinematic**
   - **BoxCollider2D** (monstruo y regalo) o **CircleCollider2D** (llave y corazón) → **Is Trigger ✔**
   - **Mover** (script) → Speed: 4
   - **Item** (script) → Kind: Monster / Key / Heart / Gift
3. Arrastra el objeto de la Hierarchy a la carpeta `Prefabs`. Así se crea el prefab.
4. **Borra el objeto de la escena.** El Spawner es el que los crea.

✅ **Comprueba:** en `Prefabs` hay 4 archivos con el ícono de cubo azul.

### Paso 9 — Spawner (Instantiate)
1. Hierarchy → clic derecho → **Create Empty** → `Spawner`.
2. Agrega el script **Spawner**.
3. Arrastra el Player al campo *Player* y los 4 prefabs a sus campos.
4. Start Interval: 1.5 · Min Interval: 0.6.

> **Cómo funciona:** es una **corrutina** que espera unos segundos con `WaitForSeconds`, elige un carril al azar y llama a `Instantiate(prefab, posición, rotación)`. Cada vez espera un poco menos, y así sube la dificultad.

### Paso 10 — GameManager y colisiones
1. Create Empty → `GameManager`. Agrega el script **GameManager**.
2. Configura: Max Lives: 3 · Keys To Win: **3** · Next Scene: **Nivel2_Calle** (lo usas en la Parte 4).

✅ **Comprueba:** en Play, toca la pantalla. Empiezan a aparecer objetos que avanzan hacia la izquierda. Si un monstruo te toca, la chica se pone roja. La Console no muestra nada en rojo.

> **Cómo funciona:** `TriggerHandler.OnTriggerEnter2D` mira qué tocaste con un `switch` (un condicional), reproduce el sonido y le avisa al GameManager (`TakeHit`, `AddKey`, `Heal`, `AddGift`). El GameManager es el único que decide si ganaste o perdiste.

### Paso 11 — Interfaz (HUD y panel final)
1. Hierarchy → **UI → Canvas**. En **Canvas Scaler**: *Scale With Screen Size*, 1920 × 1080, Match 0.5.
2. Si Unity pregunta por **TMP Essentials**, presiona **Import**.
3. Selecciona el **EventSystem**. Si aparece un botón *Replace with InputSystemUIInputModule*, presiónalo.
4. Dentro del Canvas crea:
   - **UI → Image** `IconoCorazon`, arriba a la izquierda, con el sprite del corazón. Agrégale el script **HeartPulse**.
   - **UI → Text - TextMeshPro**: `TextoVidas`, `TextoLlaves` y `TextoRegalos`, arriba.
   - **Text** `TocaParaEmpezar`, en el centro: "NIVEL 1: LA JUGUETERÍA — Toca o usa las flechas".
   - **UI → Panel** `PanelFin`, de color negro semitransparente. Dentro:
     - un Text `Titulo`;
     - un Empty `Botones`, que adentro tiene **UI → Button** `Reintentar` y `Menú`.
5. En el botón Reintentar, sección **On Click ( ) → +** → arrastra el GameManager → elige `GameManager.Restart`. En el botón Menú, elige `GameManager.GoToMenu`.
6. Agrega el script **HUDController** al Canvas y arrastra cada texto, el panel, `Botones` y el corazón a sus campos.
7. Win Message: "¡Abriste la puerta! El monstruo huyó a la calle…".

✅ **Comprueba:** las vidas bajan con cada golpe. Agarrar un corazón (con menos de 3 vidas) hace latir el ícono. Al perder aparece el panel con los botones.

### Paso 12 — El conejo en loop
1. Arrastra el conejo a la derecha, en (6, 0, 0). Order in Layer: −1.
2. Con el conejo seleccionado, en **Animation → Create** `Conejo_Loop.anim`, arrastra sus cuadros (2 o más).
3. Si tienes un solo dibujo, usa el botón rojo de grabar y anima la **Scale**: (1,1) → (1.1, 0.9) → (1,1) en 0,6 segundos.
4. Verifica **Loop Time ✔**.

✅ **Comprueba:** el conejo se mueve sin parar, también antes de empezar a jugar.

### Paso 13 — Audio
1. Pon los archivos en `Assets/Audio`. Para efectos cortos usa **.wav u .ogg**: el .mp3 se escucha con un pequeño retraso.
2. **Música:** Create Empty `Musica` → AudioSource con tu música · **Play On Awake ✔ · Loop ✔** · Volume 0.5.
3. **Efectos:** en el Player, componente **TriggerHandler**, arrastra los clips de golpe, llave, corazón y regalo.
4. **Pasos:** crea un hijo vacío del Player llamado `Pasos` → AudioSource con el sonido de pasos · Loop ✔ · Play On Awake ✘ → arrástralo al campo *Footsteps* del PlayerController.

Fuentes recomendadas (licencias verificadas):
- Música: [4 Chiptunes (Adventure)](https://opengameart.org/content/4-chiptunes-adventure) (CC0).
- Golpes, pasos y llave: packs *Impact Sounds* e *Interface Sounds* de [kenney.nl](https://kenney.nl) (CC0).
- Regalo y latido: [Pixabay](https://pixabay.com/service/terms/) (licencia Pixabay, sin créditos obligatorios).

✅ **Comprueba:** suena la música; al jugar se escuchan los pasos; cada objeto tiene su sonido.

### Paso 14 — Confeti (opcional)
1. Hierarchy → **Effects → Particle System** → `Confeti`. Configura:
   - Duration 1 · Looping ✘ · Start Lifetime 0.6–1.1 · Start Speed 3–6 · Start Size 0.08–0.16 · Gravity Modifier 1.2
   - Start Color: *Random Color* con varios colores
   - **Stop Action: Destroy**
   - Emission: Rate over Time 0, Burst de 45
   - Shape: Circle
2. Conviértelo en prefab, bórralo de la escena y arrástralo al campo *Confetti Prefab* del TriggerHandler.

---

## Parte 4 — Segundo escenario: la calle

La forma más rápida es **duplicar** el nivel 1 y cambiar solo lo que es distinto.

1. **Guarda** el Nivel 1 (Ctrl+S).
2. En Project, selecciona `Nivel1_Jugueteria` → **Ctrl+D** → renombra la copia a `Nivel2_Calle`. Ábrela.
3. **Fondo:** cambia el sprite por el de adoquines. Si es una baldosa, usa Draw Mode **Tiled**, igual que en el Paso 5.
4. **GameManager:** Keys To Win **5** · Next Scene **vacío** (es el último nivel).
5. **Spawner:** Start Interval **1.1** (más difícil).
6. **HUDController:** Win Message "¡Liberaste al conejo!".
7. Cambia el texto `TocaParaEmpezar` a "NIVEL 2: LA CALLE".
8. (Opcional) Main Camera → Background: un gris azulado, como el cielo de una calle.

**Conectar los niveles:**
- En el **Nivel 1**, el GameManager tiene que tener Next Scene = `Nivel2_Calle`, escrito exactamente igual que el nombre del archivo.
- Al juntar 3 llaves aparece el mensaje y, a los 2 segundos, se carga la calle.

**Menú (opcional pero recomendable):**
1. Crea una escena nueva `Menu` con un título y dos botones.
2. Crea un Empty con el script **MenuController**: Game Scene = `Nivel1_Jugueteria`.
3. Botón Jugar → `MenuController.Play`. Botón Salir → `MenuController.Quit`.

**Lista de escenas:** abre **File → Build Profiles → Scene List** y arrastra las escenas **en este orden**:
1. `Menu` (índice 0, la primera que abre el .exe)
2. `Nivel1_Jugueteria`
3. `Nivel2_Calle`

✅ **Comprueba:** desde el Menú, Jugar → Nivel 1 → 3 llaves → pasa solo a la Calle → 5 llaves → "¡Liberaste al conejo!". Perder en cualquiera de los dos niveles muestra Reintentar.

**Build final:** en **Build Profiles → Windows → Build**, elige la carpeta `Builds/Windows`.

✅ **Comprueba:** abre el .exe **fuera de Unity** y juega una partida ganando y otra perdiendo. **Entrega la carpeta completa**: el .exe solo no abre.

---

## Parte 5 — Modos de juego

> ⚠️ **Lee esto antes de empezar:** nada de esta parte lo pide la Pre-Entrega. Con la entrega encima, agregar otro modo de juego es la forma más segura de llegar con un juego roto. **Primero entrega el modo de 1 jugador funcionando. Después, si quieres, haz esto como Etapa 2.**

### 5.1 Un jugador (el que ya tienes)
- **Controles:** tocar la pantalla o hacer clic en un carril, o ↑ ↓ / W S.
- **Objetivo:** juntar las llaves antes de perder las 3 vidas.
- **En el celular:** funciona igual con el dedo.

### 5.2 Jugar contra la PC
**Idea:** una segunda chica, de otro color, la maneja la computadora. Las dos **compiten** en los mismos carriles: gana la primera que junta las llaves. Si una se queda sin vidas, gana la otra.

**Cómo "piensa" la PC (la IA).** Cada 0,25 segundos (su *tiempo de reacción*) mira lo que viene en cada carril y le pone puntos:

| Lo que ve cerca | Puntos para ese carril |
|---|---|
| Una llave | +3 |
| Un corazón, si le faltan vidas | +2 |
| Un regalo | +1 |
| Un monstruo a menos de 2,5 unidades | −5 |

Después se mueve **un carril** hacia el que tiene más puntos, igual que un humano con las flechas. Para que no sea perfecta (y se le pueda ganar), un 15 % de las veces "se equivoca" y no se mueve. Esos dos números son la **dificultad**: menos reacción y menos error = más difícil.

**Qué hay que cambiar en el código:**
1. `PlayerController`: agregarle un campo `bool humanInput` (si es `false`, no lee el teclado), una propiedad `public int CurrentLane => lane;` y una función `public void MoveLane(int delta)` que sume `delta` al carril (con `Mathf.Clamp`). El script de abajo **no compila** sin esas dos.
2. Crear un script nuevo `CpuBrain`, que va en la segunda chica:

```csharp
using UnityEngine;

// Elige carril mirando lo que viene. Va junto a un PlayerController con humanInput = false.
public class CpuBrain : MonoBehaviour
{
    [SerializeField] PlayerController body;
    [SerializeField] float reactionTime = 0.25f;           // menos = más difícil
    [SerializeField, Range(0f, 1f)] float mistakeChance = 0.15f;
    [SerializeField] float lookAhead = 4f;                  // cuánto "ve" hacia adelante

    float timer;

    void Update()
    {
        if (!GameManager.Instance.IsPlaying) return;
        timer -= Time.deltaTime;
        if (timer > 0f) return;
        timer = reactionTime;

        if (Random.value < mistakeChance) return;          // a veces se equivoca

        float[] lanes = body.LaneY;
        int best = body.CurrentLane;
        float bestScore = float.MinValue;
        for (int i = 0; i < lanes.Length; i++)
        {
            float score = ScoreLane(lanes[i]);
            if (score > bestScore) { bestScore = score; best = i; }
        }
        body.MoveLane((int)Mathf.Sign(best - body.CurrentLane));
    }

    float ScoreLane(float laneY)
    {
        float score = 0f;
        foreach (Item item in FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            float dx = item.transform.position.x - transform.position.x;
            bool sameLane = Mathf.Abs(item.transform.position.y - laneY) < 0.5f;
            if (!sameLane || dx < 0f || dx > lookAhead) continue;

            switch (item.Kind)
            {
                case ItemKind.Key: score += 3f; break;
                case ItemKind.Heart: score += 2f; break;
                case ItemKind.Gift: score += 1f; break;
                case ItemKind.Monster: if (dx < 2.5f) score -= 5f; break;
            }
        }
        return score;
    }
}
```

3. `GameManager`: hoy guarda **un** juego de vidas y llaves. Para dos jugadores tiene que guardar **uno por jugador** (una clase `PlayerStats { int lives; int keys; }` por cada chica), y `TriggerHandler` le tiene que decir **quién** tocó el objeto.
4. `HUDController`: mostrar dos columnas, J1 y PC.

Tiempo estimado para un principiante: **4 a 6 horas**, la mayor parte en el paso 3.

### 5.3 Sumar 2 jugadores más (multijugador local, hasta 3)
**Idea:** 3 personas en **la misma computadora**, cada una con su chica de distinto color, compitiendo por las llaves.

| Jugador | Controles | Color |
|---|---|---|
| J1 | W / S | Rosa |
| J2 | ↑ / ↓ | Celeste |
| J3 | I / K (o un joystick) | Verde |

**Qué hay que cambiar:**
1. `PlayerController`: que las teclas sean campos que eliges en el Inspector, en lugar de estar fijas en el código. Así cada chica usa las suyas.
2. Lo mismo que en 5.2: vidas y llaves **por jugador**, y que el HUD muestre las tres.
3. **Cuando dos chicas están en el mismo carril**, las dos tocan el objeto en el mismo paso de física y **las dos se llevan la llave**. Es un empate justo. Decide si te gusta antes de "arreglarlo".
4. **Joysticks:** el componente **PlayerInputManager** del Input System permite que cada jugador se una presionando un botón de su joystick. Es la forma "profesional", pero tiene su propia curva de aprendizaje.

**Lo que no va a funcionar:**
- **Multijugador en el celular:** con el toque, el juego no sabe qué dedo es de qué jugador. Esto es solo para PC con teclado o joysticks.
- **Multijugador online:** necesita un servidor o un sistema de red (Netcode for GameObjects). Es un proyecto aparte, de semanas. No lo intentes como principiante.
- **3 jugadores en 3 carriles** se va a sentir apretado. Hazle un playtest antes de decidir si quieres más carriles.

**Orden recomendado (Etapa 2, después de entregar):** primero **contra la PC** (enseña la IA y los datos por jugador). Después **2 jugadores** reutilizando eso. Al final **3 jugadores**, que es cambiar un número.

---

## Parte 6 — Errores comunes

| Síntoma | Causa | Solución |
|---|---|---|
| `NullReferenceException` al darle Play | Un campo del Inspector quedó vacío (*None*) | Mira qué script nombra la Console y arrastra lo que falta. |
| La chica atraviesa a los monstruos | Los objetos no tienen Rigidbody2D, o no tienen **Is Trigger** | Paso 8: Rigidbody2D Kinematic + Is Trigger ✔. |
| Los botones no responden | El EventSystem usa el módulo viejo | Paso 11.3: *Replace with InputSystemUIInputModule*. |
| "Parameter 'IsRunning' does not exist" | El parámetro del Animator tiene otro nombre | Tiene que ser `IsRunning`, con mayúsculas exactas. |
| La animación se reproduce una sola vez | Falta Loop Time | Selecciona el clip → **Loop Time ✔**. |
| El *pixel art* se ve borroso | El filtro no es Point | Paso 3: **Filter Mode = Point** y Compression = None. |
| No pasa al Nivel 2 | Next Scene está mal escrito, o falta en la lista de escenas | Revisa que diga `Nivel2_Calle` exacto y que esté en **Build Profiles → Scene List**. |
| `Scene 'X' couldn't be loaded` | La escena no está en la lista del build | Agrégala en **Build Profiles → Scene List**. |
| No hay sonido | La música tiene Play On Awake ✘, o el volumen está en 0 | Paso 13. |
| Los efectos suenan tarde | Son .mp3 | Usa .wav u .ogg. |
| Los sprites se ven negros | Faltan luces 2D con material Lit | Agrega **Light 2D → Global Light 2D**. |
| El .exe no abre en otra PC | Copiaste solo el .exe | Entrega la carpeta `Builds/Windows` completa. |

---

## Parte 7 — Checklist de la Pre-Entrega

Pruébalo **en el .exe**, no solo en Unity:

- [ ] **C#:** el Player responde al toque/clic y a las flechas (variables, condicionales, funciones, inputs).
- [ ] **Instantiate:** el Spawner crea monstruos, llaves, corazones y regalos.
- [ ] **Colisiones:** al chocar pasa algo visible (vidas, llaves, destello rojo, confeti).
- [ ] **Sprites:** los dos escenarios tienen fondo (juguetería y calle).
- [ ] **Animaciones:** la chica pasa de Idle a Run, y el conejo se anima en loop sin parar.
- [ ] **Audio:** suenan la música en loop y los efectos de golpe y de recolección.
- [ ] **Escenas:** Menu → Nivel 1 → Nivel 2 funciona de punta a punta.
- [ ] **Build:** la Console no muestra nada en rojo, las 3 escenas están en Build Profiles y el .exe abre en otra computadora.
