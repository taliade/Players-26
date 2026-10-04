# Rescate del Conejo — MVP Mobile (Android)

> Este documento recorta el proyecto a lo mínimo jugable en el celular.
> No reemplaza a `GDD.md`: dice **qué se queda, qué se corta y qué falta para Android**.

---

## 1. Diagnóstico de alcance

### Mecánica principal (una sola)
**Cambiar de carril tocando la pantalla** para esquivar monstruos y atrapar llaves.

### Core loop
```
TOCAR un carril  ->  la chica se desliza a ese carril + SFX al tocar algo  ->  llave: +1 puntaje / monstruo: -1 vida
```

### Lo que el proyecto tiene hoy vs. lo que necesita el MVP
| Elemento | Hoy | MVP mobile | Por qué |
|---|---|---|---|
| Chica (Player) con Idle/Run | ✔ | **Se queda** | Es el jugador. |
| Monstruo | ✔ | **Se queda** | Único obstáculo. |
| Llave | ✔ | **Se queda** | Único coleccionable = puntaje y meta. |
| Corazón (cura) | ✔ | **Se corta** | Suaviza la derrota: esconde si la dificultad está bien o mal. |
| Regalo + confeti | ✔ | **Se corta** | No cambia ninguna decisión del jugador. |
| `HeartPulse`, pasos en loop | ✔ | **Se corta** | Pulido, no mecánica. |
| Nivel 2 (calle) | ✔ | **Se corta** | Primero hay que validar que el nivel 1 es divertido. |
| Menú | ✔ | **Opcional** | El MVP arranca con "Toca para empezar". |
| Conejo en loop | ✔ | **Se queda** | Lo pide la consigna (objeto animado en loop). |

**Cómo recortar sin tocar código** (en la escena `Nivel1_Jugueteria`):
- Spawner → `Heart Chance` = **0**. *No* dejes vacío `Heart Prefab` con la probabilidad en más de 0: `Instantiate(null)` tira error. Solo `Gift Prefab` acepta quedar vacío.
- Spawner → `Gift Prefab` vacío.
- GameManager → `Next Scene` vacío (así no salta al nivel 2).

---

## 2. One-Page GDD técnico (mobile)

**High concept:** Tocá un carril para esquivar a los monstruos azules y juntar 5 llaves antes de perder tus 3 vidas.

**Plataforma y orientación:** Android, **Landscape (horizontal)**.
- Los objetos vienen de derecha a izquierda: el horizontal da más distancia para reaccionar.
- Con el celular en horizontal y dos manos, los dos pulgares llegan a toda la altura de la pantalla, y los 3 carriles quedan a la altura de los pulgares.
- **Riesgo a probar:** si tocás con el pulgar derecho, tapás los monstruos que vienen. Pruébalo con 3 personas. Si pasa, limita el toque a la mitad izquierda de la pantalla.
- **iOS:** desde Windows se puede exportar el proyecto Xcode, pero compilarlo e instalarlo requiere **Mac + Xcode**. Si no tienes Mac, el MVP es solo Android. Dilo en la entrega en vez de prometerlo.

**Controles (New Input System):**
| Entrada | Acción | Binding |
|---|---|---|
| Tocar la pantalla | La chica va al carril más cercano a la altura del dedo | `<Pointer>/press` + `<Pointer>/position` |
| Flechas ↑ ↓ / W S | Un carril arriba / abajo (para probar en PC) | `<Keyboard>/...` |

`<Pointer>` cubre mouse y pantalla táctil con el mismo código: se prueba en PC y funciona en el celular.

**Win / Lose:**
- **Gana:** 5 llaves → panel "¡Liberaste al conejo!".
- **Pierde:** 3 golpes (0 vidas) → panel "El monstruo te atrapó".
- **Reinicio:** botón **Reintentar** → `SceneManager.LoadScene(buildIndex actual)`. Un toque, sin menú en el medio.

**Entidades mínimas:** Chica (jugador) · Monstruo (obstáculo) · Llave (coleccionable/meta) · Conejo (decorado en loop).

---

## 3. Hoja de ruta (4 fases)

> El generador (`Rescate → 1. Generar escenas`) hace todo esto solo. **Hazlo igual una vez a mano en una escena aparte.** Si el docente pregunta "¿por qué el monstruo es Kinematic?" y no sabes responder, el generador te perjudica.

### Fase 1 — Greybox (mecánica pura)
1. Cámara: **Orthographic**, Size 4. Fondo de un color liso.
2. Chica = cuadrado blanco. Rigidbody2D **Dynamic**, Gravity Scale **0**, Freeze Rotation Z. BoxCollider2D.
3. Monstruo = cuadrado rojo. Llave = círculo amarillo. Collider con **Is Trigger** y Rigidbody2D **Kinematic**.
4. Agrega `PlayerController` y `Mover`. Prueba **solo** que la chica cambia de carril y los objetos cruzan la pantalla.
5. **Criterio de salida:** con formas grises, ¿dan ganas de jugar 30 segundos? Si no, el arte no lo va a arreglar.

### Fase 2 — Interacción & Triggers
1. `Item` en cada prefab (Kind = Monster / Key).
2. `TriggerHandler` en la chica: `OnTriggerEnter2D` lee el `Item` y avisa al `GameManager`.
3. **Trigger vs Collision:**
   - `OnTriggerEnter2D` = "atravesó": lo usas para recoger y para recibir daño.
   - `OnCollisionEnter2D` = "chocó y rebotó" (los dos colliders sólidos): no lo usa este juego.
   - Si los dos colliders son sólidos y uno es Kinematic, empuja a la chica fuera del carril.
4. **Criterio de salida:** con 0 vidas el juego se congela; con 5 llaves se congela con victoria. Todavía sin UI: alcanza con `Debug.Log`.

### Fase 3 — Mobile Input (Android)
El input ya es táctil (`<Pointer>`). Lo que **falta** en el proyecto para que funcione bien en un celular real:
1. **Android Build Support** instalado desde Unity Hub, con OpenJDK y SDK/NDK.
2. *Build Profiles → Android → Switch Platform.*
3. *Player Settings → Resolution and Presentation → Default Orientation:* **Landscape Left**. Si dejas Auto Rotation, el juego gira a vertical y los carriles quedan mal.
4. **60 FPS:** por defecto Unity limita a **30 FPS en mobile**. Agrega en `GameManager.Awake()`:
   ```csharp
   Application.targetFrameRate = 60;
   ```
5. **Notch / bordes redondeados:** el HUD puede quedar debajo de la cámara frontal. Crea un panel hijo del Canvas que ocupe toda la pantalla, mueve el HUD adentro y ponle esto:
   ```csharp
   using UnityEngine;

   // Ajusta el panel al área segura de la pantalla (fuera del notch).
   [RequireComponent(typeof(RectTransform))]
   public class SafeArea : MonoBehaviour
   {
       void Awake()
       {
           RectTransform rt = GetComponent<RectTransform>();
           Rect safe = Screen.safeArea;
           Vector2 min = safe.position;
           Vector2 max = safe.position + safe.size;
           min.x /= Screen.width;  min.y /= Screen.height;
           max.x /= Screen.width;  max.y /= Screen.height;
           rt.anchorMin = min;
           rt.anchorMax = max;
       }
   }
   ```
6. Prueba en el **Device Simulator** (*Window → General → Device Simulator*) y **después en un celular de verdad**. Activa la depuración USB y usa *Build And Run*. El simulador no te dice si el pulgar tapa a los monstruos.

### Fase 4 — UI & Game Loop
1. Canvas con **Scale With Screen Size** 1920×1080, Match 0.5.
2. HUD: **Vidas** (arriba a la izquierda) y **Llaves x/5** (arriba a la derecha), en los rincones lejos de los pulgares.
3. Texto "Toca para empezar": el primer toque arranca la partida (`GameManager.StartGame`).
4. Panel final con **Reintentar** → `GameManager.Restart` (`SceneManager.LoadScene`).
5. EventSystem con **InputSystemUIInputModule**. Si queda el `StandaloneInputModule` viejo, el botón no responde al toque.
6. Botón grande: mínimo **~1 cm** en pantalla (unos 150×150 px a 1080p). Un botón chico se erra con el pulgar.

---

## 4. Arquitectura de scripts (MVP)

| Script | Responsabilidad única |
|---|---|
| `PlayerController` | Lee el toque/teclado y mueve a la chica al carril elegido. Le dice al Animator si corre o está quieta. |
| `Mover` | Mueve un objeto de derecha a izquierda a velocidad fija. Lo destruye al salir de cámara. |
| `Item` | Solo datos: dice qué es el objeto (Monstruo, Llave…). No tiene lógica. |
| `Spawner` | Cada cierto tiempo hace `Instantiate` de un monstruo o una llave en un carril al azar. Acorta el intervalo para subir la dificultad. |
| `TriggerHandler` | Detecta qué tocó la chica (`OnTriggerEnter2D`), suena el SFX y avisa al GameManager. No cuenta vidas: delega. |
| `GameManager` | Dueño de las reglas: vidas, llaves, victoria/derrota y reinicio. No dibuja UI: avisa con eventos. |
| `HUDController` | Escucha al GameManager y dibuja vidas, llaves y el panel final. No cambia el estado del juego. |

**Fuera del MVP:** `HeartPulse`, `MenuController` y el confeti.

**Dónde el código no cumple del todo lo que predica (para que lo sepas antes que el docente):**
- `PlayerController` también maneja el sonido de pasos. Eso no es "mover a la chica". En el MVP los pasos se cortan.
- `Spawner` depende de `PlayerController` para saber dónde están los carriles. Es un acoplamiento aceptable (una sola fuente de verdad), pero si mañana hay dos jugadores, se rompe.
- `TriggerHandler` mezcla audio, feedback visual y reglas. Para 4 casos con `switch` está bien; con 10 tipos de objeto, no.

---

## 5. Checklist antes de instalar el APK
- [ ] La orientación está fija en Landscape y no gira.
- [ ] Un toque en un carril mueve a la chica a ese carril, sin retraso.
- [ ] Llaves suben el contador; monstruos bajan vidas; cada choque suena.
- [ ] 5 llaves = victoria; 0 vidas = derrota; Reintentar reinicia con un toque.
- [ ] El HUD no queda tapado por el notch.
- [ ] Corre fluido (60 FPS) en un celular real, no solo en el simulador.
- [ ] Consola sin errores rojos antes del build.
