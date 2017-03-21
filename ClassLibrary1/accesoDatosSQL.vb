Imports System.Data.SqlClient
Imports EmailSender.emailSender


Public Class accesoDatosSQL
    Private Shared conexion As New SqlConnection
    Private Shared comando As New SqlCommand

    Public Shared Function conectar() As String
        Try
            conexion.ConnectionString = "Data Source=tcp:hads2017.database.windows.net,1433;Initial Catalog=HADS17_TAREAS;Persist Security Info=True;User ID=hads17;Password=Camellos17"
            conexion.Open()
        Catch ex As Exception
            Return "ERROR DE CONEXIÓN: " + ex.Message
        End Try
        Return "CONEXION OK"
    End Function

    Public Shared Sub cerrarConexion()
        conexion.Close()
    End Sub

    Public Shared Function insertar(ByVal correo As String, ByVal nombre As String, ByVal apellidos As String, ByVal pregunta As String, ByVal respuesta As String, ByVal dni As String, ByVal pass As String, ByVal rol As String) As String
        Dim NumConf As Integer
        Dim sender As New EmailSender.emailSender
        ' Crear numero aleatorio
        Randomize()
        NumConf = CLng(Rnd() * 9000000) + 1000000
        Dim st = "insert into Usuarios (email,nombre,apellidos,pregunta,respuesta, dni, numconfir, confirmado, tipo, pass) values ('" & correo & "','" & nombre & "','" & apellidos & "', '" & pregunta & "','" & respuesta & "','" & dni & "','" & NumConf & "','0','" & rol & "','" & pass & "')"
        Dim numregs As Integer
        comando = New SqlCommand(st, conexion)
        Try
            numregs = comando.ExecuteNonQuery()
            If (numregs = 1 And sender.enviarEmail(correo, NumConf)) Then
                Return "Registro completado. Revisa tu correo para activar tu cuenta."
            Else
                Return "Ha ocurrido un error inesperado. Inténtalo de nuevo"
            End If
        Catch ex As Exception
            Return ex.Message
        End Try
        Return numregs
    End Function


    Public Shared Function activar(ByVal correo As String, ByVal num As Integer) As Integer
        'Respuestas: 0-> Correcto; 1-> Error en consulta de BD, 2-> URL/numconfirmacion modificado
        Dim resultadoConsulta As SqlDataReader
        Dim comando As New SqlCommand
        Dim comando1 As New SqlCommand
        Dim numConf As New Integer
        Dim numregs As New Integer
        Try
            Dim consulta = "select * from Usuarios where email='" + correo + "'"
            comando = New SqlCommand(consulta, conexion)
            resultadoConsulta = comando.ExecuteReader()
        Catch ex As Exception
            Return 1
        End Try
        If (resultadoConsulta.Read) Then
            numConf = resultadoConsulta.Item("numconfir")
            resultadoConsulta.Close() 'IMPORTANTE! -> No se pueden tener más de 2 DataReader's en una misma conexión
            If (num = numConf) Then
                Dim update = "update Usuarios set confirmado='1' where email='" + correo + "'"
                Try
                    comando1 = New SqlCommand(update, conexion)
                    comando1.ExecuteNonQuery()
                    Return 0
                Catch ex As Exception
                    Return 1
                End Try
            Else
                Return 2
            End If
        Else
            Return 2
        End If
    End Function

    Public Shared Function login(ByVal correo As String, ByVal password As String) As Integer
        'Devuelve -2 si datos incorrectos, -1 en caso de error, 0 si es alumno y 1 si es profesor
        Dim comando As New SqlCommand
        Dim resultadoConsulta As SqlDataReader
        Dim tipo As String
        Dim respuesta As Integer
        Try
            Dim consulta = "select tipo from Usuarios where email='" + correo + "' and pass='" + password + "' and confirmado='1'"
            comando = New SqlCommand(consulta, conexion)
            resultadoConsulta = comando.ExecuteReader()
            If (resultadoConsulta.Read) Then
                tipo = resultadoConsulta.Item("tipo")
                If tipo.Equals("P") Then
                    respuesta = 1
                ElseIf tipo.Equals("A") Then
                    respuesta = 0
                Else
                    respuesta = -1
                End If
            Else
                respuesta = -2
            End If
            Return respuesta
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Public Shared Function comprobarCorreo(ByVal correo As String) As Integer
        Dim comando As New SqlCommand
        Dim numregs As New Integer
        Try
            Dim consulta = "select count(*) from Usuarios where email='" + correo + "'"
            comando = New SqlCommand(consulta, conexion)
            numregs = comando.ExecuteScalar()
            Return numregs
        Catch ex As Exception
            Return -1
        End Try
    End Function

    Public Shared Function obtenerPreguntaRespuesta(ByVal correo As String) As String()
        Dim resultadoConsulta As SqlDataReader
        Dim comando As New SqlCommand
        Dim respuesta(2) As String
        Try
            Dim consulta = "select pregunta,respuesta from Usuarios where email='" + correo + "'"
            comando = New SqlCommand(consulta, conexion)
            resultadoConsulta = comando.ExecuteReader()
            If resultadoConsulta.Read Then
                respuesta(1) = resultadoConsulta.Item("pregunta")
                respuesta(2) = resultadoConsulta.Item("respuesta")
            End If
            Return respuesta
        Catch ex As Exception
            Return Nothing
        End Try
    End Function


    Public Shared Function guardarPassword(ByVal password As String, ByVal correo As String) As Integer
        'Respuestas: 0-> Correcto; 1-> Error en consulta de BD'
        Dim comando As New SqlCommand
        Dim comando1 As New SqlCommand

        Dim update = "update Usuarios set pass='" + password + "' where email='" + correo + "'"
        Try
            comando1 = New SqlCommand(update, conexion)
            comando1.ExecuteNonQuery()
            Return 0
        Catch ex As Exception
            Return 1
        End Try
    End Function

End Class
