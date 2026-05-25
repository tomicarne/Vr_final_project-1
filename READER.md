# CARÁNIMAS — Contexto para Claude Code

## Qué es este proyecto
Escape room psicológico en VR para Meta Quest 3. Unity 6 + Meta XR Core SDK.
El jugador es Sigmund Campbell, quien perdió su Persona y Ego tras una trepanación.
Debe resolver 4 puzzles para abrir 4 candados y escapar de su propia mente.

## La estructura del juego
- Habitación única de 5x5m
- 4 candados en la puerta norte, cada uno se abre con una llave específica
- Cada llave está escondida dentro de un puzzle
- Al insertar la llave en el candado, cae al suelo y revela lore

## Puzzle 1 — Espejo de la Verdad (dificultad 2/10)
- Jugador jala sábana del espejo con grip
- El vidrio se agrieta, aparece la pregunta "¿Qué es lo que más escondes?"
- Por el cuarto hay palabras escondidas, la correcta es CULPA
- Al seleccionarla: flash rojo, marco del espejo se abre como compartimento
- La llave 1 cae al suelo
- Jugador lleva llave al Candado 1 → cae → se revela lore

## Puzzle 2 — Las Voces Reprimidas (dificultad 5/10)
- 4 velas negras con símbolos: llama=ira, gota=tristeza, ojo=miedo, cruz=culpa
- 4 cuadros con horas: ira=3, miedo=9, tristeza=6, culpa=12
- Reloj parado en las 9 → emoción dominante es MIEDO
- Orden de encendido: sentido horario desde las 9 → miedo, culpa, ira, tristeza
- Error: llama azul 0.5s → todas las velas se apagan → reinicio
- Secuencia correcta: vela del miedo derrite su cera completamente
- Llave 2 aparece atrapada en la cera → jugador espera que enfríe o la rompe
- Jugador lleva llave al Candado 2 → cae → se revela lore

## Puzzle 3 — El Diario Roto (dificultad 8/10)
- 20 páginas dispersas por el cuarto, solo 4 tienen subrayado rojo en el canto
- Pista del espejo: "Lee solo lo que hiere"
- Palabras subrayadas: juzgo, niego, oculto, temo → iniciales: J·N·O·T
- Pista en la pared: "La sombra siempre habla al revés" → JNOT invertido = TONJ
- Libro con alfabeto simbólico: T=M, O=I, N=E, J=D → TONJ = MIED
- Letra O pintada en la madera del escritorio, visible solo con linterna → MIEDO
- Panel de letras deslizables en el escritorio → escribir MIEDO abre cajón secreto
- Dentro del cajón: llave 3 + foto de Sigmund con Alma + orden de alejamiento
- Jugador lleva llave al Candado 3 → cae → se revela lore

## Puzzle 4 — La Silueta Fragmentada (dificultad 6/10)
- Marco de madera 1.8x0.9m en pared oeste con silueta y 4 ranuras vacías
- 4 piezas escondidas con acertijos poéticos en notas:
  - Ojo: "La sombra siempre observa desde donde no miras" → detrás del cuadro torcido
  - Corazón: "Late bajo aquello que más pesa" → bajo el objeto más pesado
  - Boca: "Lo no dicho vive donde las palabras mueren" → dentro de un libro hueco
  - Mano: "Tu sombra toca lo que abandonas" → bajo la silla rota
- Cada pieza al colocarse emite un tono bajo y la sombra proyectada crece
- Al colocar el Corazón (última pieza): pecho de la silueta se abre como compartimento
- La sombra proyectada cambia: ya no es adulto, es la silueta de una niña (Alma, 7 años)
- NO hay música dramática aquí — el silencio hace el trabajo
- Dentro del compartimento: llave 4
- Al insertar llave 4: los 4 candados caen en secuencia
- La puerta se entreabre → oscuridad del otro lado → voz final

## Sistema de candados (PadlockSystem)
- 4 candados oxidados en la puerta norte
- Cada candado tiene un símbolo grabado: verdad, emoción, miedo, identidad
- Al insertar llave correcta: animación de apertura + sonido metálico único por candado
- El candado cae físicamente al suelo
- Al caer el último: la puerta emite luz fría desde el umbral y se entreabre

## Scripts esperados
- Scripts/Player/: PlayerController.cs, HandInteraction.cs, KeyInteraction.cs
- Scripts/Puzzles/: MirrorPuzzle.cs, CandlePuzzle.cs, DiaryPuzzle.cs, SilhouettePuzzle.cs
- Scripts/Systems/: AudioManager.cs, ProgressTracker.cs, PadlockSystem.cs, LoreManager.cs
- Scripts/UI/: PauseMenu.cs, HintSystem.cs

## Sistema de hints
- Si el jugador no interactúa por 5 minutos → susurro en audio espacial 3D
- El susurro viene de la dirección del puzzle activo
- La voz es la de la Sombra
- NO hay texto de pista, NO hay flechas

## Plataforma y rendimiento
- Meta Quest 3 standalone
- 72 FPS mínimo, 90 FPS ideal
- Locomoción: teleport principal + smooth locomotion opcional
- UI 100% diegética, sin HUD, sin tutoriales en pantalla