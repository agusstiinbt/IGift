Datos importantes:

AspNetRoleClaims: Almacena los claims asociados a cada rol en tu aplicaci�n. Los claims son afirmaciones sobre el usuario, como permisos o roles.
AspNetRoles: Almacena los roles de usuario en tu aplicaci�n. Los roles agrupan usuarios que comparten permisos o responsabilidades comunes.
AspNetUserClaims: Almacena los claims asociados a cada usuario en tu aplicaci�n. Estos claims pueden contener informaci�n adicional sobre el usuario, como su nombre, correo electr�nico, etc.
AspNetUserLogins: Utilizada para almacenar informaci�n sobre los proveedores de inicio de sesi�n externos asociados a cada usuario. Por ejemplo, si un usuario inicia sesi�n mediante Google, se almacenar�a aqu� la informaci�n relacionada con esa autenticaci�n externa.
AspNetUserRoles: Tabla de relaci�n muchos a muchos que relaciona usuarios con roles. Indica qu� roles tiene asignados cada usuario.
AspNetUsers: Almacena informaci�n b�sica sobre los usuarios de tu aplicaci�n, como su nombre de usuario, correo electr�nico, contrase�a (o referencia a un proveedor externo), etc.
AspNetUserTokens: Utilizada para almacenar tokens de autenticaci�n para cada usuario. Por ejemplo, tokens de restablecimiento de contrase�a, tokens de autenticaci�n de dos factores, etc.
AspNetDeviceCodes: Utilizada en OAuth 2.0 para almacenar c�digos de dispositivo para el flujo de autorizaci�n del dispositivo.
Keys: Utilizada para almacenar claves de seguridad, como claves de autenticaci�n de dos factores.
PersistedGrants: Utilizada para almacenar tokens OAuth 2.0 y datos relacionados con la autorizaci�n persistente.



�C�mo trabajamos en la aplicaci�n?

La l�gica tratamos de dejarla siempre en el c�digo C# antes que en la base de datos.

Con los modelos de Infrastructure, fijarse cu�les son los campos que tiene en su respectiva tabla y despu�s de eso llenarlos siempre desde el front.

En las tablas de identity como por ejemplo Users, hay campos que debemos fijarnos que permiten un Null, entonces esos campos no hace falta llenarlos al momento de una creaci�n de usuario. Pero sin embargo
hay que fijarse en qu� m�todos de su clase servicio (UserService en este caso) s� son necesarios completarlos


Explicaci�n de ClockSkew
Por defecto, el TokenValidationParameters.ClockSkew est� configurado con un valor de 5 minutos para permitir una peque�a discrepancia
en el tiempo entre el emisor del token y el validador. Esto es �til en entornos de producci�n donde puede haber ligeras diferencias
en el tiempo de los servidores. Sin embargo, para pruebas locales y ciertos escenarios espec�ficos, es �til establecerlo en TimeSpan.Zero 
para asegurar que la expiraci�n del token sea exacta.


Uso de BackgroundJob.Enqueue
Prop�sito: BackgroundJob.Enqueue se utiliza para ejecutar una tarea en segundo plano de forma as�ncrona, fuera del hilo principal de la aplicaci�n. Esto es adecuado para tareas que pueden llevar mucho tiempo y no necesitan completarse de inmediato.
Ejemplo de uso: Enviar correos electr�nicos, procesar archivos, realizar c�lculos extensivos.
Persistencia y reintentos: Los trabajos encolados con BackgroundJob.Enqueue se persisten en una base de datos (o alg�n tipo de almacenamiento) y Hangfire maneja autom�ticamente los reintentos en caso de fallos.
No bloquea el hilo principal: Permite que el flujo de la aplicaci�n contin�e sin esperar a que la tarea se complete.
Escalabilidad: Puede distribuir trabajos entre varios servidores y manejar cargas de trabajo altas.

Uso de await
Prop�sito: await se utiliza para esperar de forma as�ncrona la finalizaci�n de una tarea dentro del mismo contexto o hilo. Permite que el hilo se libere mientras espera, pero contin�a dentro del mismo flujo de ejecuci�n.
Ejemplo de uso: Esperar una respuesta de una API, leer archivos, realizar operaciones de base de datos.
Contexto de ejecuci�n: await mantiene el contexto de sincronizaci�n, lo que significa que despu�s de que la tarea se completa, el c�digo contin�a ejecut�ndose en el mismo hilo.
No hay persistencia o reintentos autom�ticos: A diferencia de BackgroundJob.Enqueue, await no maneja la persistencia o reintentos en caso de fallos. Si se necesita, debe implementarse manualmente.
Bloqueo del flujo de la aplicaci�n: Aunque no bloquea el hilo, await espera a que la tarea termine antes de continuar con la ejecuci�n del c�digo posterior.

Diferencias Clave
Contexto de ejecuci�n: await mantiene el contexto de sincronizaci�n y continua en el mismo hilo, mientras que BackgroundJob.Enqueue ejecuta la tarea en un contexto completamente separado.
Persistencia y manejo de fallos: Hangfire maneja la persistencia y los reintentos autom�ticos de tareas fallidas, mientras que await no lo hace.
Idoneidad: BackgroundJob.Enqueue es m�s adecuado para tareas largas y no cr�ticas que pueden ejecutarse independientemente del flujo principal de la aplicaci�n. await es adecuado para tareas que deben completarse antes de continuar, manteniendo el flujo l�gico del programa.





Colecciones:

Usar la interfaz m�s gen�rica posible:

public IEnumerable<string> GetNames()
{
    return new List<string> { "Alice", "Bob", "Charlie" 
};


Preferir IEnumerable<T> para iteraci�n:

Si el prop�sito del m�todo es simplemente iterar sobre una colecci�n, usa IEnumerable<T>.
Por qu�: Es la interfaz m�s gen�rica y cubre la mayor�a de los casos de uso para la iteraci�n.


Usar ICollection<T> para manipulaci�n de colecciones:


// Para iteraci�n
public IEnumerable<string> GetNames()
{
    return new List<string> { "Alice", "Bob", "Charlie" };
}

// Para manipulaci�n b�sica
public ICollection<string> GetEditableNames()
{
    return new List<string> { "Alice", "Bob", "Charlie" };
}

// Para acceso por �ndice
public IList<string> GetIndexedNames()
{
    return new List<string> { "Alice", "Bob", "Charlie" };
}



Mapeos:
Veamos el siguiente ejemplo 

Esto es mucho m�s rapido que un mapeo. Ver c�digo fuente con explicaci�n
 Expression<Func<Domain.Entities.Pedidos, PedidosResponse>> expression = e => new PedidosResponse
            {
                Descripcion = e.Descripcion,
                Moneda = e.Moneda,
                Monto = e.Monto,
            };

            var filtro = new PedidosFilter(request.SearchString);
            if (request.OrderBy?.Any() != true)
            {
                var response = await _unitOfWork.Repository<Domain.Entities.Pedidos>().Entities
                    .Specify(filtro)
                    .Select(expression)
                    .ToPaginatedListAsync(request.PageNumber, request.PageSize);
                return response;
            }