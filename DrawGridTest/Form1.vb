Public Class Form1
    Public Function GetRandom(ByVal Min As Integer, ByVal Max As Integer) As Integer
        Dim Generator As System.Random = New System.Random()
        Return Generator.Next(Min, Max)
    End Function
    Private Sub Form1_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        DrawGrid1.addColumn("Nombre", 150, 0)
        DrawGrid1.addColumn("Email", 200, 1)
        DrawGrid1.addColumn("Telefono", 200, 2)


        Dim dt As DataTable = New DataTable()

        dt.Columns.Add("Nombre")
        dt.Columns.Add("Email")
        dt.Columns.Add("Telefono")


        For i As Integer = 0 To 10000
            dt.Rows.Add("Javier", "yinda@gmail.com", GetRandom(99999, 100000000))
            dt.Rows.Add("Pedro", "yinda@sdfsdf.com", GetRandom(99999, 100000000))
            dt.Rows.Add("Juan Ignacio Perez", "yinda_djiia_q929ssl_aoso@gmail.com", "39493122919191")
            dt.Rows.Add("Javier", "yinda@gmail.com", "39493919191")
            dt.Rows.Add("Pedro", "yinda@gmail.com", "sdfsd")
            dt.Rows.Add("Juan Ignacio Perez", "yindaaoso@gmail.com", GetRandom(99999, 100000000))
            dt.Rows.Add("Javier", "yinda@gmail.com", "222")
            dt.Rows.Add("Pedro", "yinda@sdfsdf.com", "39493919232191")
            dt.Rows.Add("Juan Ignacio Perez", "yinda_djiia_q929ssl_aoso@gmail.com", "39493122919191")
            dt.Rows.Add("Javier", "yinda@gmail.com", "39493919191")
            dt.Rows.Add("Pedro", "yinda@gmail.com", "sdfsd")
        Next i

        DrawGrid1.SetData(dt)

    End Sub
End Class
