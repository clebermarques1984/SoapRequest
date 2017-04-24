Imports System.IO
Imports System.Net
Imports System.Xml

Module Module1
	Sub Main()
		Dim retorno As String = GetResponse("http://200.152.194.6:8888/consultas.asmx",
									  "http://tempuri.org/ConsultaConsolidadaPagamentoPorData",
									  My.Resources.App.SOAPEnvelope)
		Console.WriteLine(retorno)
		Console.Read()
	End Sub

	''' <summary>
	''' Create a soap webrequest to [Url]
	''' </summary>
	''' <returns></returns>
	Public Function CreateWebRequest(url As String, soapAction As String) As HttpWebRequest
		Dim request As HttpWebRequest = WebRequest.Create(url)
		request.Headers.Add($"SOAPAction:{soapAction}")
		request.ContentType = "text/xml; charset=utf-8"
		request.Accept = "text/xml"
		request.Method = "POST"
		Return request
	End Function

	Public Function GetResponse(url As String, soapAction As String, soapEnvelope As String) As String
		Try
			Dim request As HttpWebRequest = CreateWebRequest(url, soapAction)
			Dim soapEnvelopeXml As New XmlDocument()
			soapEnvelopeXml.LoadXml(My.Resources.App.SOAPEnvelope)
			Using stream As Stream = request.GetRequestStream
				soapEnvelopeXml.Save(stream)
			End Using
			Using response As WebResponse = request.GetResponse()
				Using rd As New StreamReader(response.GetResponseStream())
					Dim soapResult As String = rd.ReadToEnd()
					Return soapResult
				End Using
			End Using
		Catch ex As Exception
			Return ex.Message
		End Try
	End Function
End Module


Public Class Entity
	Private cmd As String

	Private Shared _instance As Entity

	Private Sub New(tableName As String)
		cmd = $"select * from {tableName}"
	End Sub

	Public Shared Function Create(tableName As String) As Entity
		If _instance Is Nothing Then
			Return New Entity(tableName)
		Else
			Return _instance
		End If
	End Function

	Public Function Where(predicate As String) As Entity
		cmd += $" where {predicate}"
		Return Me
	End Function

	Public Function Order(collumns() As String) As Entity
		cmd += $" order by {String.Join(",", collumns)}"
		Return Me
	End Function

	Public Function GetCmdText() As String
		Return cmd
	End Function
End Class