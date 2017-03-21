CREATE PROCEDURE getAsignaturasProfesor

@profesor nvarchar(50)
AS

select distinct codigoasig
from GruposClase
where codigo in (select codigogrupo from ProfesoresGrupo where email=@profesor)
