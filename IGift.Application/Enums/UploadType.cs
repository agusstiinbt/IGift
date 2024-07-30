using System.ComponentModel;

namespace IGift.Application.Enums
{
    /// <summary>
    /// Este Enum se utiliza para saber en qué directorio guardaremos un tipo de archivo, habiendo tipos como Imagenes, Documentos y sus sub-variedades
    /// </summary>
    public enum UploadType : byte
    {
        /// <summary>
        /// Imágenes solamente de Productos
        /// </summary>
        [Description(@"Images\Products")]
        Product,
        /// <summary>
        /// Imagenes solamente de perfil de usuarios
        /// </summary>
        [Description(@"Images\ProfilePictures")]
        ProfilePicture,
        /// <summary>
        /// Todo tipo de documentos
        /// </summary>
        [Description(@"Documents")]//TODO luego se puede separar entre pdf, excel, etc?
        Document,
        /// <summary>
        /// Imágenes solamente de los locales adheridos
        /// </summary>
        [Description(@"Images\LocalesAdheridos")]
        LocalesAdheridos
    }
}
