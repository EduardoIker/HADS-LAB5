CREATE PROCEDURE getDedicacionTareas 
	@codasig nvarchar(10),
	@email nvarchar(50)
AS
	SELECT ET.CodTarea, ET.HEstimadas, ET.HReales
	FROM EstudiantesTareas AS ET INNER JOIN TareasGenericas AS TG ON ET.CodTarea=TG.Codigo
	WHERE TG.CodAsig=@codasig AND ET.Email=@email