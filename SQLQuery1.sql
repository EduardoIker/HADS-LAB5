select codigoasig, email
from GruposClase as gc inner join ProfesoresGrupo as pg on gc.codigo=pg.codigogrupo 
