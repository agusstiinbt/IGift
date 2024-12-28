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
        [Description(@"Documents\PDF")]
        DocumentPDF,
        [Description(@"Documents\Excel")]
        DocumentExcel,
        [Description(@"Documents\Txt")]
        DocumentTxt,
        /// <summary>
        /// Imágenes solamente de los locales adheridos
        /// </summary>
        [Description(@"Images\LocalesAdheridos")]
        LocalesAdheridos
    }
}
