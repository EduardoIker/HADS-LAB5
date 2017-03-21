Imports AccesoDatos.accesoDatosSQL

Public Class WebForm1
    Inherits System.Web.UI.Page

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        conectar()
    End Sub

    Protected Sub Page_Unload(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Unload
        cerrarConexion()
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Dim resultado As Integer
        resultado = login(TextBox1.Text, TextBox2.Text)
        If (resultado = 0) Then
            Session.Contents("correo") = TextBox1.Text
            Session.Contents("tipo") = "A"
            Response.Redirect("Alumno.aspx")
        ElseIf (resultado = 1) Then
            Session.Contents("correo") = TextBox1.Text
            Session.Contents("tipo") = "P"
            Response.Redirect("Profesor.aspx")
        ElseIf (resultado = -2) Then
            Label2.Text = "Datos de acceso incorrectos. Inténtalo de nuevo."
        Else
            Label2.Text = "Ha ocurrido un problema inesperado. Inténtalo de nuevo."
        End If
    End Sub


End Class