Este es un pequeño proyecto que te permite enviar múltiples peticiones HTTP a una URL y ver cómo responde el servidor. como como un “bombardeo de solicitudes” para probar el rendimiento de manera sencilla.

Qué hace?

Envía solicitudes POST/GET a intervalos regulares.

Transmite la posición del “jugador” como un dato de ejemplo.

Muestra todo en la consola de Unity 

Cómo ejecutarlo?

Abre el proyecto en Unity.

En la escena, selecciona el objeto Api ( ApiClient).

Cambia la Base Url por tu endpoint (ejemplo: http://localhost:5005/server).

Si lo deseas, arrastra tu Player a Player Transform para enviar su posición.

Ajusta el Auto Interval (segundos entre peticiones, por ejemplo: 0.5 o 1).

Dale Play y observa la consola: verás los Auto POST → ....

 Mis archivos clave:

ApiClient.cs → cliente HTTP (intervalo, POST/GET).

ServerData.cs → payload simple (x, y, z).

PlayerController.cs → movimiento básico para generar datos.

GameManager.cs → imprime lo que llega (opcional).

Hola memo:V
