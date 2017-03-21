Imports System.Data.SqlClient
Imports System.Xml
Imports Newtonsoft.Json
Imports System.IO
Imports System.Net

Public Class ExportarTareas
    Inherits System.Web.UI.Page

    Dim conClsf As SqlConnection = New SqlConnection("Data Source=tcp:hads2017.database.windows.net,1433;Initial Catalog=HADS17_TAREAS;Persist Security Info=True;User ID=hads17;Password=Camellos17")
    Dim dap1 As New SqlDataAdapter
    Dim dap2 As New SqlDataAdapter
    Dim dst As New DataSet
    Dim tbl As New DataTable
    Dim row As DataRow
    Dim vista As DataView

    Protected Sub Page_Load(ByVal sender As Object, ByVal e As System.EventArgs) Handles Me.Load
        If Page.IsPostBack Then
            dst = Session("datos")
            vista = Session("vista")
        Else
            'Para coger las asignaturas
            dap1 = New SqlDataAdapter("select distinct codigoasig from GruposClase where codigo in (select codigogrupo from ProfesoresGrupo where email='" + Session("correo") + "')", conClsf)
            Dim bld As New SqlCommandBuilder(dap1)
            dap1.Fill(dst, "Asignaturas")
            tbl = dst.Tables("Asignaturas")
            DropDownList1.DataSource = tbl
            DropDownList1.DataTextField = "codigoasig" 'Nombre de la columna de la tabla
            DropDownList1.DataBind()
            'Obtener las tareas y almacenarlas en otra DataTable del DataSet
            dap2 = New SqlDataAdapter("select * from TareasGenericas", conClsf)
            Dim bld2 As New SqlCommandBuilder(dap2)
            dap2.Fill(dst, "Tareas")
            'Vista para obtener las tareas de la asignatura seleccionada
            vista = New DataView(dst.Tables("Tareas"))
            vista.RowFilter = "CodAsig=" & "'" & DropDownList1.SelectedValue & "'"
            GridView1.DataSource = vista
            GridView1.DataBind()
            Session("datos") = dst
            Session("vista") = vista
        End If
    End Sub

    Protected Sub DropDownList1_SelectedIndexChanged(sender As Object, e As EventArgs) Handles DropDownList1.SelectedIndexChanged
        'Vista para obtener las tareas de la asignatura seleccionada
        vista = New DataView(dst.Tables("Tareas"))
        vista.RowFilter = "CodAsig=" & "'" & DropDownList1.SelectedValue & "'"
        GridView1.DataSource = vista
        GridView1.DataBind()
        'Session("vista") = vista
        Label5.Text = ""
    End Sub

    Protected Sub Button1_Click(sender As Object, e As EventArgs) Handles Button1.Click
        Try
            'Codigo para crear un fichero XML que contenga las tareas
            tbl = vista.ToTable
            'XmlWriterSettings.
            Dim settings As XmlWriterSettings = New XmlWriterSettings()
            settings.Indent = True ' añade sangrias al resultado
            'XmlWriter.
            Dim fileLoc = "App_Data/" & DropDownList1.SelectedValue & ".xml"
            Using writer As XmlWriter = XmlWriter.Create(Server.MapPath(fileLoc), settings)
                'Raiz del XML
                writer.WriteStartDocument()
                writer.WriteStartElement("tareas")
                writer.WriteAttributeString("xmlns", DropDownList1.SelectedValue.ToLower, Nothing, "http://ji.ehu.es/" + DropDownList1.SelectedValue.ToLower) 'CAMBIAR
                For Each row As DataRow In tbl.Rows
                    writer.WriteStartElement("tarea")
                    writer.WriteElementString("codigo", row.Item(0).ToString)
                    writer.WriteElementString("descripcion", row.Item(1).ToString)
                    writer.WriteElementString("hestimadas", row.Item(3).ToString)
                    writer.WriteElementString("explotacion", row.Item(4).ToString)
                    writer.WriteElementString("tipotarea", row.Item(5).ToString)
                    writer.WriteEndElement()
                Next
                writer.WriteEndElement()
                writer.WriteEndDocument()
            End Using
            'Descargar el archivo 
            'Dim myWebClient As New WebClient()
            'My.Computer.Network.DownloadFile(Server.MapPath(fileLoc), "HAS.xml")
            Label5.ForeColor = Drawing.Color.Black
            Label5.Text = "Tareas exportadas correctamente (" & fileLoc & ")"
        Catch ex As Exception
            Label5.ForeColor = Drawing.Color.Red
            Label5.Text = "Error en la exportación. Inténtalo de nuevo."
        End Try
    End Sub

    Protected Sub Button2_Click(sender As Object, e As EventArgs) Handles Button2.Click
        Try
            tbl = vista.ToTable
            tbl.Columns.Remove("CodAsig") 'Quitar la columna CodAsig
            Dim jsonString = JsonConvert.SerializeObject(tbl)
            Dim fileLoc = "App_Data/" & DropDownList1.SelectedValue & ".json"
            'Crea o reescribe el fichero
            Dim fs As FileStream = File.Create(Server.MapPath(fileLoc))
            'Añadir el texto -> String JSON
            Dim jsonText As Byte() = New UTF8Encoding(True).GetBytes(jsonString)
            fs.Write(jsonText, 0, jsonText.Length)
            fs.Close()
            Label5.ForeColor = Drawing.Color.Black
            Label5.Text = "Tareas exportadas correctamente (" & fileLoc & ")"
        Catch ex As Exception
            Label5.ForeColor = Drawing.Color.Red
            Label5.Text = ex.Message
        End Try
    End Sub

    Protected Sub LinkButton1_Click(sender As Object, e As EventArgs) Handles LinkButton1.Click
        Session.Abandon()
        Response.Redirect("Inicio.aspx")
    End Sub
End Class