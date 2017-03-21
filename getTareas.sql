CREATE PROCEDURE getTareas 
	@asignatura nvarchar(10)
AS
	select *
	from TareasGenericas
	where CodAsig=@asignatura