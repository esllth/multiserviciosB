from pathlib import Path
import re

root = Path(r'C:\Users\Usuario\OneDrive\Documents\GitHub\multiserviciosB')
excluded = {'.git', '.vs', 'bin', 'obj', 'node_modules', '.nuget', 'lib'}
extensions = {'.cs', '.cshtml', '.js', '.json', '.md', '.txt', '.sql', '.xml', '.config', '.props', '.targets'}

replacements = {
    'accion': 'acción', 'Accion': 'Acción', 'acciones': 'acciones', 'acciones': 'acciones',
    'administracion': 'administración', 'Administracion': 'Administración',
    'agregacion': 'agregación', 'Agregacion': 'Agregación',
    'algun': 'algún', 'Algun': 'Algún', 'algunos': 'algunos', 'algunos': 'algunos',
    'anos': 'años', 'Anos': 'Años',
    'area': 'área', 'Area': 'Área', 'areas': 'áreas', 'Areas': 'Áreas',
    'autenticacion': 'autenticación', 'Autenticacion': 'Autenticación',
    'automatico': 'automático', 'Automatico': 'Automático', 'automatica': 'automática', 'Automatica': 'Automática',
    'busqueda': 'búsqueda', 'Busqueda': 'Búsqueda',
    'calificacion': 'calificación', 'Calificacion': 'Calificación',
    'categoria': 'categoría', 'Categoria': 'Categoría', 'categorias': 'categorías', 'Categorias': 'Categorías',
    'comunicacion': 'comunicación', 'Comunicacion': 'Comunicación',
    'configuracion': 'configuración', 'Configuracion': 'Configuración',
    'confirmacion': 'confirmación', 'Confirmacion': 'Confirmación',
    'conexion': 'conexión', 'Conexion': 'Conexión',
    'cotizacion': 'cotización', 'Cotizacion': 'Cotización', 'cotizaciones': 'cotizaciones',
    'descripcion': 'descripción', 'Descripcion': 'Descripción', 'descripciones': 'descripciones',
    'direccion': 'dirección', 'Direccion': 'Dirección', 'direcciones': 'direcciones',
    'diseno': 'diseño', 'Diseno': 'Diseño',
    'documentacion': 'documentación', 'Documentacion': 'Documentación',
    'diagnostico': 'diagnóstico', 'Diagnostico': 'Diagnóstico',
    'eliminacion': 'eliminación', 'Eliminacion': 'Eliminación',
    'emision': 'emisión', 'Emision': 'Emisión',
    'especificacion': 'especificación', 'Especificacion': 'Especificación',
    'esta': 'está', 'Esta': 'Está', 'estan': 'están', 'Estan': 'Están', 'estas': 'estas',
    'evaluacion': 'evaluación', 'Evaluacion': 'Evaluación',
    'fabricacion': 'fabricación', 'Fabricacion': 'Fabricación',
    'facturacion': 'facturación', 'Facturacion': 'Facturación',
    'fotografica': 'fotográfica', 'Fotografica': 'Fotográfica', 'fotografico': 'fotográfico', 'Fotografico': 'Fotográfico',
    'garantia': 'garantía', 'Garantia': 'Garantía',
    'historico': 'histórico', 'Historico': 'Histórico', 'historica': 'histórica', 'Historica': 'Histórica',
    'informacion': 'información', 'Informacion': 'Información',
    'inspeccion': 'inspección', 'Inspeccion': 'Inspección',
    'instalacion': 'instalación', 'Instalacion': 'Instalación',
    'instruccion': 'instrucción', 'Instruccion': 'Instrucción',
    'maquina': 'máquina', 'Maquina': 'Máquina', 'maquinas': 'máquinas', 'Maquinas': 'Máquinas',
    'mas': 'más', 'Mas': 'Más',
    'metodo': 'método', 'Metodo': 'Método', 'metodos': 'métodos', 'Metodos': 'Métodos',
    'minimo': 'mínimo', 'Minimo': 'Mínimo', 'minima': 'mínima', 'Minima': 'Mínima',
    'modulo': 'módulo', 'Modulo': 'Módulo', 'modulos': 'módulos', 'Modulos': 'Módulos',
    'movimiento': 'movimiento', 'Movimiento': 'Movimiento',
    'numero': 'número', 'Numero': 'Número', 'numeros': 'números', 'Numeros': 'Números',
    'opcion': 'opción', 'Opcion': 'Opción', 'opciones': 'opciones', 'Opciones': 'Opciones',
    'operacion': 'operación', 'Operacion': 'Operación', 'operaciones': 'operaciones',
    'pagina': 'página', 'Pagina': 'Página', 'paginas': 'páginas', 'Paginas': 'Páginas',
    'pais': 'país', 'Pais': 'País', 'paises': 'países', 'Paises': 'Países',
    'peticion': 'petición', 'Peticion': 'Petición',
    'politica': 'política', 'Politica': 'Política', 'politicas': 'políticas', 'Politicas': 'Políticas',
    'podra': 'podrá', 'Podra': 'Podrá', 'podran': 'podrán', 'Podran': 'Podrán',
    'produccion': 'producción', 'Produccion': 'Producción',
    'prohibido': 'prohibido',
    'proximo': 'próximo', 'Proximo': 'Próximo', 'proxima': 'próxima', 'Proxima': 'Próxima',
    'publicacion': 'publicación', 'Publicacion': 'Publicación',
    'razon': 'razón', 'Razon': 'Razón',
    'recepcion': 'recepción', 'Recepcion': 'Recepción',
    'recomendacion': 'recomendación', 'Recomendacion': 'Recomendación',
    'registro': 'registro',
    'relacion': 'relación', 'Relacion': 'Relación',
    'reporte': 'reporte',
    'requisicion': 'requisición', 'Requisicion': 'Requisición',
    'revision': 'revisión', 'Revision': 'Revisión',
    'seleccion': 'selección', 'Seleccion': 'Selección',
    'sesion': 'sesión', 'Sesion': 'Sesión',
    'situacion': 'situación', 'Situacion': 'Situación',
    'solicitud': 'solicitud',
    'solucion': 'solución', 'Solucion': 'Solución',
    'tambien': 'también', 'Tambien': 'También',
    'telefono': 'teléfono', 'Telefono': 'Teléfono',
    'tecnico': 'técnico', 'Tecnico': 'Técnico', 'tecnicos': 'técnicos', 'Tecnicos': 'Técnicos',
    'termino': 'término', 'Termino': 'Término',
    'titulo': 'título', 'Titulo': 'Título',
    'ubicacion': 'ubicación', 'Ubicacion': 'Ubicación',
    'ultimo': 'último', 'Ultimo': 'Último', 'ultima': 'última', 'Ultima': 'Última',
    'unico': 'único', 'Unico': 'Único', 'unica': 'única', 'Unica': 'Única',
    'util': 'útil', 'Util': 'Útil',
    'validacion': 'validación', 'Validacion': 'Validación',
    'verificacion': 'verificación', 'Verificacion': 'Verificación',
    'vigencia': 'vigencia',
    'dia': 'día', 'Dia': 'Día', 'dias': 'días', 'Dias': 'Días',
    'codigo': 'código', 'Codigo': 'Código', 'codigos': 'códigos', 'Codigos': 'Códigos',
    'deberia': 'debería', 'Deberia': 'Debería',
    'logica': 'lógica', 'Logica': 'Lógica',
    'logico': 'lógico', 'Logico': 'Lógico',
    'medida': 'medida',
    'periodo': 'período', 'Periodo': 'Período',
    'publico': 'público', 'Publico': 'Público', 'publica': 'pública', 'Publica': 'Pública',
    'responsable': 'responsable',
    'rapido': 'rápido', 'Rapido': 'Rápido', 'rapida': 'rápida', 'Rapida': 'Rápida',
    'segun': 'según', 'Segun': 'Según',
    'solo': 'solo', 'Solo': 'Solo',
    'tambien': 'también', 'Tambien': 'También',
    'valido': 'válido', 'Valido': 'Válido', 'valida': 'válida', 'Valida': 'Válida',
}

word_pattern = re.compile(r'(?<![\w])(' + '|'.join(sorted(map(re.escape, replacements), key=len, reverse=True)) + r')(?![\w])')

def replace_words(text):
    return word_pattern.sub(lambda m: replacements[m.group(1)], text)

def replace_cs_text(text):
    # Replace words in quoted strings and comments only.
    text = re.sub(r'(?<!\\)(["\'])(?:(?!\1).)*\1', lambda m: replace_words(m.group(0)), text, flags=re.DOTALL)
    text = re.sub(r'//[^\r\n]*', lambda m: replace_words(m.group(0)), text)
    text = re.sub(r'/\*.*?\*/', lambda m: replace_words(m.group(0)), text, flags=re.DOTALL)
    text = re.sub(r'<!--.*?-->', lambda m: replace_words(m.group(0)), text, flags=re.DOTALL)
    # Replace visible text between HTML tags in Razor files.
    text = re.sub(r'>([^<>]+)<', lambda m: '>' + replace_words(m.group(1)) + '<', text)
    return text

changed = []
for path in root.rglob('*'):
    if not path.is_file() or path.suffix.lower() not in extensions:
        continue
    if any(part in excluded for part in path.parts):
        continue
    try:
        old = path.read_text(encoding='utf-8')
    except UnicodeDecodeError:
        continue
    new = replace_words(old) if path.suffix.lower() in {'.md', '.txt'} else replace_cs_text(old)
    if new != old:
        path.write_text(new, encoding='utf-8', newline='')
        changed.append(str(path.relative_to(root)))

print('\n'.join(changed))
print(f'CHANGED_FILES={len(changed)}')
