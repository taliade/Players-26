# Rescate del Conejo — GDD (MVP Pre-Entrega)

> Rellena la plantilla del curso con este contenido. Lo marcado con **(TU DECISIÓN)** lo tienes que confirmar tú.

## High concept
Una chica esquiva en tres carriles a los monstruos azules que avanzan como zombies de *Plants vs Zombies* y junta 5 llaves para liberar a su conejo.

## Core loop
| Acción | Feedback | Consecuencia |
|---|---|---|
| Tocar/clic en un carril | La chica cambia de carril corriendo (animación Run) | — |
| Tocar una llave/corazón/regalo | Sonido de recolección y el objeto desaparece | +1 llave / +1 vida / +1 regalo |
| Chocar con un monstruo | Sonido de golpe y el monstruo desaparece | −1 vida |
| 5 llaves | Panel "¡Liberaste al conejo!" | Victoria → Reintentar |
| 0 vidas | Panel "El monstruo te atrapó" | Derrota → Reintentar (recarga la escena) |

## ¿A quién está dirigido? (TU DECISIÓN)
- **Edad:** 8+ · **Género:** todos · **Necesidad:** partidas cortas (1–2 min) que se entienden sin tutorial.

## ¿Qué quiero lograr?
1. Que se entienda en 5 segundos: tocar un carril = ir ahí.
2. Una partida de menos de 2 minutos, con reinicio inmediato.
3. Que el build ejecutable corra sin errores.

## ¿De qué se trata?
- **Género:** arcade de carriles / esquivar.
- **Narrativa:** el monstruo azul encerró al conejo; la chica junta las llaves para abrir la jaula.
- **Jugabilidad:** 3 carriles horizontales. Los objetos vienen de derecha a izquierda.
- **Temporalidad:** sin tiempo límite; termina por victoria o derrota.
- **Música:** loop de fondo pixel/chiptune + 4 SFX (golpe, llave, corazón, regalo).
- **Estilo:** pixel art, horizontal (Landscape).

## ¿En qué juegos te inspiraste?
1. *Plants vs Zombies* — carriles horizontales y enemigos que avanzan de derecha a izquierda.
2. *Subway Surfers* — cambiar de carril para esquivar y juntar cosas.
3. **(TU DECISIÓN)** — uno que hayas jugado de verdad, y qué tomas de él.

## Personajes (3)
1. **Chica** — jugadora. Animaciones Idle y Run.
2. **Monstruo azul** — el peligro. Es un prefab que el Spawner instancia muchas veces.
3. **Conejo** — el objetivo. Está enjaulado a la derecha con una animación en loop.

Las llaves, los corazones y los regalos son **objetos**, no personajes.

## Escenas
1. **Juego** — una única escena con: cartel "Toca para empezar", la partida y el panel de fin (Ganaste/Perdiste + Reintentar).
2. Menú — **fuera del MVP**. Agregarlo solo si sobra tiempo y la consigna lo pide.

## Fuera del MVP (recortado a propósito)
Menú, niveles, récord guardado, power-ups, partículas, animación de muerte, ataque del jugador.

## Prioridad de recorte si falta tiempo
Los **regalos** se recortan primero: no cambian ninguna decisión del jugador.
Para sacarlos, deja vacío `Gift Prefab` en el Spawner y borra el texto del HUD. No hace falta tocar código.
