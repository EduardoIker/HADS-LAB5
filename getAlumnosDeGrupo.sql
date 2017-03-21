CREATE PROCEDURE getAlumnosDeGrupo 
	@correo nvarchar(50), 
	@asignatura nvarchar(10)
AS
	SELECT EG.Email
	FROM (EstudiantesGrupo AS EG INNER JOIN ProfesoresGrupo AS PG ON EG.Grupo=PG.codigogrupo) INNER JOIN GruposClase AS GC ON GC.codigo=PG.codigogrupo
	WHERE GC.codigoasig=@asignatura AND PG.email=@correo