Public Class TareasProfesor
    Inherits System.Web.UI.Page

    Protected Sub LinkButton1_Click(sender As Object, e As EventArgs) Handles LinkButton1.Click
        Session.Abandon()
        Response.Redirect("Inicio.aspx")
    End Sub
End Class