using System.Globalization;
using System.IO.Compression;
using System.Text;
using MultiservicioB.Models;

namespace MultiservicioB.Services
{
    public static class CotizacionPdfService
    {
        public static byte[] Crear(Cotizacion cotizacion)
        {
            var cliente = cotizacion.Cliente;
            var nombreCliente = $"{cliente?.Nombre} {cliente?.Apellidos}".Trim();
            var monto = cotizacion.MontoPresupuesto.HasValue ? FormatearCrc(cotizacion.MontoPresupuesto.Value) : "Pendiente";
            var adelanto = cotizacion.RequiereAdelanto && cotizacion.PorcentajeAdelanto.HasValue
                ? $"{cotizacion.PorcentajeAdelanto.Value}% ({FormatearCrc(CalcularAdelanto(cotizacion))})"
                : "No aplica";

            var lineas = new List<string>
            {
                "MULTISERVICIO BOLIVAR",
                $"Cotizacion #{cotizacion.IdCotizacion}",
                $"Fecha de emision: {DateTime.Now:dd/MM/yyyy hh:mm tt}",
                "",
                "DATOS DEL CLIENTE",
                $"Nombre: {nombreCliente}",
                $"Cedula/identificacion: {cliente?.Identificacion}",
                $"Correo: {cliente?.Correo ?? "No indicado"}",
                "",
                "DETALLE DE LA COTIZACION",
                $"Servicio: {cotizacion.TipoServicio?.Nombre ?? "No indicado"}",
                $"Descripcion: {cotizacion.Descripcion ?? "No indicada"}",
                $"Monto presupuestado: {monto}",
                $"Adelanto solicitado: {adelanto}",
                $"Forma de pago aceptada: {DescribirFormaPago(cotizacion.FormaPagoAceptada)}",
                "",
                "CONDICIONES DE PAGO",
                "El cliente reconoce haber leido y aceptado los Terminos y Condiciones de Cotizacion y Pago.",
                "Los precios, plazos de fabricacion, instalacion y entrega se establecen de comun acuerdo.",
                "El cliente se compromete a realizar los pagos en las fechas acordadas.",
                "En caso de incumplimiento, la empresa podra suspender trabajos pendientes, retener entregas",
                "cuando corresponda y gestionar el cobro por medios administrativos o legales procedentes",
                "conforme a la legislacion de la Republica de Costa Rica.",
                "",
                "SELECCION DE PAGO PARA FIRMA",
                "[  ] Pago completo por adelantado",
                "[  ] Pago del adelanto acordado y saldo final pendiente segun acuerdo",
                "",
                "ACEPTACION",
                "Declaro que he leido y acepto las condiciones de esta cotizacion y las condiciones de pago.",
                "",
                "Firma ficticia de referencia:",
                "",
                "",
                "Firma del cliente: ________________________________",
                "Nombre: __________________________________________",
                "Cedula: __________________________________________",
                "Fecha: ___________________________________________"
            };

            return CrearPdfSimple(lineas, CargarFirmaFicticia(), CargarLogo());
        }

        private static decimal CalcularAdelanto(Cotizacion cotizacion)
        {
            if (!cotizacion.MontoPresupuesto.HasValue || !cotizacion.PorcentajeAdelanto.HasValue)
            {
                return 0;
            }

            return cotizacion.MontoPresupuesto.Value * cotizacion.PorcentajeAdelanto.Value / 100m;
        }

        private static string DescribirFormaPago(string? formaPago)
        {
            return formaPago switch
            {
                "Completo" => "Pago completo por adelantado",
                "AdelantoSaldo" => "Pago del adelanto acordado y saldo final pendiente",
                _ => "Pendiente de seleccion"
            };
        }

        private static string FormatearCrc(decimal monto)
        {
            return $"CRC {monto.ToString("N2", CultureInfo.InvariantCulture)}";
        }

        private static byte[] CrearPdfSimple(IReadOnlyList<string> lineas, FirmaImagen? firma, FirmaImagen? logo)
        {
            var contenido = new StringBuilder();
            if (logo != null)
            {
                contenido.AppendLine("q");
                contenido.AppendLine("120 0 0 46 430 728 cm");
                contenido.AppendLine("/Logo Do");
                contenido.AppendLine("Q");
            }

            contenido.AppendLine("BT");
            contenido.AppendLine("/F2 13 Tf");
            contenido.AppendLine("50 742 Td");

            for (var i = 0; i < lineas.Count && i < 40; i++)
            {
                var linea = lineas[i];
                if (i == 1)
                {
                    contenido.AppendLine("/F1 10 Tf");
                }
                else if (EsEncabezado(linea))
                {
                    contenido.AppendLine("/F2 10 Tf");
                }
                else if (!string.IsNullOrWhiteSpace(linea))
                {
                    contenido.AppendLine("/F1 10 Tf");
                }

                contenido.AppendLine($"({Escapar(Normalizar(linea))}) Tj");
                contenido.AppendLine("0 -16 Td");
            }

            contenido.AppendLine("ET");
            if (firma != null)
            {
                contenido.AppendLine("q");
                contenido.AppendLine("175 0 0 30 50 210 cm");
                contenido.AppendLine("/Firma Do");
                contenido.AppendLine("Q");
            }

            var stream = Encoding.ASCII.GetBytes(contenido.ToString());
            var xObjects = new List<string>();
            var siguienteImagen = 7;
            if (firma != null)
            {
                xObjects.Add($"/Firma {siguienteImagen++} 0 R");
            }

            if (logo != null)
            {
                xObjects.Add($"/Logo {siguienteImagen++} 0 R");
            }

            var recursosImagen = xObjects.Count == 0 ? "" : $" /XObject << {string.Join(" ", xObjects)} >>";
            var objetos = new List<byte[]>
            {
                ToAscii("1 0 obj\n<< /Type /Catalog /Pages 2 0 R >>\nendobj\n"),
                ToAscii("2 0 obj\n<< /Type /Pages /Kids [3 0 R] /Count 1 >>\nendobj\n"),
                ToAscii($"3 0 obj\n<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 4 0 R /F2 5 0 R >>{recursosImagen} >> /Contents 6 0 R >>\nendobj\n"),
                ToAscii("4 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Times-Roman >>\nendobj\n"),
                ToAscii("5 0 obj\n<< /Type /Font /Subtype /Type1 /BaseFont /Times-Bold >>\nendobj\n"),
                ToAscii($"6 0 obj\n<< /Length {stream.Length} >>\nstream\n{contenido}endstream\nendobj\n")
            };

            var numeroObjetoImagen = 7;
            if (firma != null)
            {
                using var imagenObj = new MemoryStream();
                WriteAscii(imagenObj, $"{numeroObjetoImagen++} 0 obj\n<< /Type /XObject /Subtype /Image /Width {firma.Ancho} /Height {firma.Alto} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /FlateDecode /Length {firma.RgbComprimido.Length} >>\nstream\n");
                imagenObj.Write(firma.RgbComprimido);
                WriteAscii(imagenObj, "\nendstream\nendobj\n");
                objetos.Add(imagenObj.ToArray());
            }

            if (logo != null)
            {
                using var logoObj = new MemoryStream();
                WriteAscii(logoObj, $"{numeroObjetoImagen++} 0 obj\n<< /Type /XObject /Subtype /Image /Width {logo.Ancho} /Height {logo.Alto} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /FlateDecode /Length {logo.RgbComprimido.Length} >>\nstream\n");
                logoObj.Write(logo.RgbComprimido);
                WriteAscii(logoObj, "\nendstream\nendobj\n");
                objetos.Add(logoObj.ToArray());
            }

            using var pdf = new MemoryStream();
            WriteAscii(pdf, "%PDF-1.4\n");
            var offsets = new List<long> { 0 };

            foreach (var objeto in objetos)
            {
                offsets.Add(pdf.Position);
                pdf.Write(objeto);
            }

            var xref = pdf.Position;
            WriteAscii(pdf, "xref\n");
            WriteAscii(pdf, $"0 {objetos.Count + 1}\n");
            WriteAscii(pdf, "0000000000 65535 f \n");
            foreach (var offset in offsets.Skip(1))
            {
                WriteAscii(pdf, $"{offset:0000000000} 00000 n \n");
            }

            WriteAscii(pdf, "trailer\n");
            WriteAscii(pdf, $"<< /Size {objetos.Count + 1} /Root 1 0 R >>\n");
            WriteAscii(pdf, "startxref\n");
            WriteAscii(pdf, xref.ToString(CultureInfo.InvariantCulture));
            WriteAscii(pdf, "\n%%EOF");

            return pdf.ToArray();
        }

        private static bool EsEncabezado(string linea)
        {
            return linea is "DATOS DEL CLIENTE" or
                "DETALLE DE LA COTIZACION" or
                "CONDICIONES DE PAGO" or
                "SELECCION DE PAGO PARA FIRMA" or
                "ACEPTACION";
        }

        private static FirmaImagen? CargarFirmaFicticia()
        {
            var ruta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "FirmaCotizacionYAcuerdoDePago.png");

            return File.Exists(ruta) ? LeerPngRgbaComoRgb(ruta) : null;
        }

        private static FirmaImagen? CargarLogo()
        {
            var ruta = Path.Combine(
                Directory.GetCurrentDirectory(),
                "wwwroot",
                "images",
                "Logo",
                "logo.png");

            return File.Exists(ruta) ? LeerPngRgbaComoRgb(ruta) : null;
        }

        private static FirmaImagen LeerPngRgbaComoRgb(string ruta)
        {
            var bytes = File.ReadAllBytes(ruta);
            var posicion = 8;
            var ancho = 0;
            var alto = 0;
            using var idat = new MemoryStream();

            while (posicion < bytes.Length)
            {
                var longitud = LeerInt32BigEndian(bytes, posicion);
                posicion += 4;
                var tipo = Encoding.ASCII.GetString(bytes, posicion, 4);
                posicion += 4;

                if (tipo == "IHDR")
                {
                    ancho = LeerInt32BigEndian(bytes, posicion);
                    alto = LeerInt32BigEndian(bytes, posicion + 4);
                    var colorType = bytes[posicion + 9];
                    if (colorType != 6)
                    {
                        throw new InvalidOperationException("La firma ficticia debe ser un PNG RGBA.");
                    }
                }
                else if (tipo == "IDAT")
                {
                    idat.Write(bytes, posicion, longitud);
                }
                else if (tipo == "IEND")
                {
                    break;
                }

                posicion += longitud + 4;
            }

            idat.Position = 0;
            using var zlib = new ZLibStream(idat, CompressionMode.Decompress);
            using var datos = new MemoryStream();
            zlib.CopyTo(datos);

            var raw = datos.ToArray();
            var stride = ancho * 4;
            var rgba = new byte[alto * stride];
            var anterior = new byte[stride];
            var cursor = 0;

            for (var y = 0; y < alto; y++)
            {
                var filtro = raw[cursor++];
                var fila = new byte[stride];
                Array.Copy(raw, cursor, fila, 0, stride);
                cursor += stride;
                Desfiltrar(fila, anterior, filtro, 4);
                Array.Copy(fila, 0, rgba, y * stride, stride);
                anterior = fila;
            }

            var rgb = new byte[ancho * alto * 3];
            for (int i = 0, j = 0; i < rgba.Length; i += 4, j += 3)
            {
                var alpha = rgba[i + 3] / 255d;
                rgb[j] = ComponerBlanco(rgba[i], alpha);
                rgb[j + 1] = ComponerBlanco(rgba[i + 1], alpha);
                rgb[j + 2] = ComponerBlanco(rgba[i + 2], alpha);
            }

            using var comprimido = new MemoryStream();
            using (var deflate = new ZLibStream(comprimido, CompressionLevel.SmallestSize, leaveOpen: true))
            {
                deflate.Write(rgb, 0, rgb.Length);
            }

            return new FirmaImagen(ancho, alto, comprimido.ToArray());
        }

        private static void Desfiltrar(byte[] fila, byte[] anterior, byte filtro, int bpp)
        {
            for (var i = 0; i < fila.Length; i++)
            {
                var izquierda = i >= bpp ? fila[i - bpp] : 0;
                var arriba = anterior[i];
                var arribaIzquierda = i >= bpp ? anterior[i - bpp] : 0;
                fila[i] = filtro switch
                {
                    1 => (byte)(fila[i] + izquierda),
                    2 => (byte)(fila[i] + arriba),
                    3 => (byte)(fila[i] + ((izquierda + arriba) / 2)),
                    4 => (byte)(fila[i] + Paeth(izquierda, arriba, arribaIzquierda)),
                    _ => fila[i]
                };
            }
        }

        private static byte Paeth(int a, int b, int c)
        {
            var p = a + b - c;
            var pa = Math.Abs(p - a);
            var pb = Math.Abs(p - b);
            var pc = Math.Abs(p - c);
            return (byte)(pa <= pb && pa <= pc ? a : pb <= pc ? b : c);
        }

        private static byte ComponerBlanco(byte color, double alpha)
        {
            return (byte)Math.Round(color * alpha + 255 * (1 - alpha));
        }

        private static int LeerInt32BigEndian(byte[] bytes, int offset)
        {
            return (bytes[offset] << 24) |
                   (bytes[offset + 1] << 16) |
                   (bytes[offset + 2] << 8) |
                   bytes[offset + 3];
        }

        private static byte[] ToAscii(string texto)
        {
            return Encoding.ASCII.GetBytes(texto);
        }

        private static void WriteAscii(Stream stream, string texto)
        {
            stream.Write(ToAscii(texto));
        }

        private static string Escapar(string texto)
        {
            return texto.Replace("\\", "\\\\").Replace("(", "\\(").Replace(")", "\\)");
        }

        private static string Normalizar(string texto)
        {
            return texto
                .Replace("á", "a").Replace("é", "e").Replace("í", "i").Replace("ó", "o").Replace("ú", "u")
                .Replace("Á", "A").Replace("É", "E").Replace("Í", "I").Replace("Ó", "O").Replace("Ú", "U")
                .Replace("ñ", "n").Replace("Ñ", "N").Replace("₡", "CRC ");
        }

        private sealed record FirmaImagen(int Ancho, int Alto, byte[] RgbComprimido);
    }
}
