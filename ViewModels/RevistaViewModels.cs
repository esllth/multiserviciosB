using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;
using MultiservicioB.DTOs;

namespace MultiservicioB.ViewModels
{
    public class RevistaViewModel
    {
        public string Titulo { get; set; } = "Multiservicio Bolivar";
        public string Descripcion { get; set; } = "Fabricacion, refrigeracion y mantenimiento industrial con una ejecucion limpia y profesional.";
        public string ImagenPrincipal { get; set; } = "/images/Revista/Hero.png";
        public string Encabezado { get; set; } = "Revista de trabajos";
        public string Subtitulo { get; set; } = "Soluciones industriales con acabado limpio.";
        public List<RevistaTarjetaViewModel> Tarjetas { get; set; } = CrearTarjetasIniciales();
        public List<HorarioDTO> HorariosDisponibles { get; set; } = new();

        public static List<RevistaTarjetaViewModel> CrearTarjetasIniciales() =>
        [
            new() { Titulo = "Fabricacion a medida", Descripcion = "Componentes industriales con precision, orden y acabado profesional.", Imagen = "/images/Revista/Revista1.jpg", TextoEnlace = "Solicitar cotizacion" },
            new() { Titulo = "Instalacion tecnica", Descripcion = "Montajes limpios para operacion continua y mantenimiento sencillo.", Imagen = "/images/Revista/Revista5.jpg", TextoEnlace = "Ver servicio" },
            new() { Titulo = "Acabado industrial", Descripcion = "Detalles funcionales pensados para resistencia, limpieza y durabilidad.", Imagen = "/images/Revista/Revista8.jpg", TextoEnlace = "Ver detalle" },
            new() { Titulo = "Servicio especializado", Descripcion = "Diagnostico y ejecucion con criterio tecnico en campo.", Imagen = "/images/Revista/Revista2.jpg", TextoEnlace = "Coordinar visita" },
            new() { Titulo = "Equipos instalados", Descripcion = "Integracion sobria para espacios de trabajo exigentes.", Imagen = "/images/Revista/Revista10.png", TextoEnlace = "Consultar" },
            new() { Titulo = "Mantenimiento", Descripcion = "Intervenciones ordenadas para conservar rendimiento y seguridad.", Imagen = "/images/Revista/Revista3.png", TextoEnlace = "Programar" }
        ];
    }

    public class RevistaTarjetaViewModel
    {
        public string Titulo { get; set; } = "";
        public string Descripcion { get; set; } = "";
        public string Imagen { get; set; } = "";
        public string TextoEnlace { get; set; } = "";
    }

    public class RevistaEditarViewModel
    {
        [Required, StringLength(100)]
        [Display(Name = "Título principal")]
        public string Titulo { get; set; } = "";

        [Required, StringLength(300)]
        [Display(Name = "Descripción principal")]
        public string Descripcion { get; set; } = "";

        public string ImagenPrincipalActual { get; set; } = "";

        [Display(Name = "Nueva imagen principal")]
        public IFormFile? ImagenPrincipal { get; set; }

        [Required, StringLength(80)]
        [Display(Name = "Nombre de la sección")]
        public string Encabezado { get; set; } = "";

        [Required, StringLength(150)]
        [Display(Name = "Subtítulo")]
        public string Subtitulo { get; set; } = "";

        public List<RevistaTarjetaEditarViewModel> Tarjetas { get; set; } = new();
    }

    public class RevistaTarjetaEditarViewModel
    {
        [Required, StringLength(80)]
        public string Titulo { get; set; } = "";

        [Required, StringLength(250)]
        public string Descripcion { get; set; } = "";

        public string ImagenActual { get; set; } = "";

        [Display(Name = "Nueva imagen")]
        public IFormFile? NuevaImagen { get; set; }

        [Required, StringLength(50)]
        [Display(Name = "Texto del botón")]
        public string TextoEnlace { get; set; } = "";
    }
}
