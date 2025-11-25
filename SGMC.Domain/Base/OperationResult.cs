namespace SGMC.Domain.Base
{
    public class OperationResult
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public object? Datos { get; set; }
        public List<string> Errores { get; } = new();

        public static OperationResult Exito(
            string mensaje = "Operación realizada con éxito.",
            object? datos = null) =>
            new()
            {
                Exitoso = true,
                Mensaje = mensaje,
                Datos = datos
            };

        public static OperationResult Fallo(
            string mensaje = "La operación ha fallado.",
            List<string>? errores = null)
        {
            var result = new OperationResult
            {
                Exitoso = false,
                Mensaje = mensaje
            };

            if (errores is { Count: > 0 })
                result.Errores.AddRange(errores);

            return result;
        }

        public void AgregarError(string error) => Errores.Add(error);
    }

    public class OperationResult<T>
    {
        public bool Exitoso { get; set; }
        public string Mensaje { get; set; } = string.Empty;
        public T? Datos { get; set; }
        public List<string> Errores { get; } = new();

        public static OperationResult<T> Exito(
            T? datos = default,
            string mensaje = "Operación realizada con éxito.") =>
            new()
            {
                Exitoso = true,
                Mensaje = mensaje,
                Datos = datos
            };

        public static OperationResult<T> Fallo(
            string mensaje = "La operación ha fallado.",
            List<string>? errores = null)
        {
            var result = new OperationResult<T>
            {
                Exitoso = false,
                Mensaje = mensaje
            };

            if (errores is { Count: > 0 })
                result.Errores.AddRange(errores);

            return result;
        }

        public void AgregarError(string error) => Errores.Add(error);
    }
}