using Grpc.Core;
using Newtonsoft.Json;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Reflection.PortableExecutable;

namespace ProtoBuf.Services
{
  public class EmployeeService : EmployeeCrud.EmployeeCrudBase
  {
    public override Task<InsertReply> Insert(InsertRequest request, ServerCallContext context)
    {
      var cn = new SqlConnection("Data Source=.;initial catalog=Test; User ID=sa;Password=1234;");

      var cmd = new SqlCommand("INSERT INTO dbo.MyTable ( FirstName, Lastname, NationalCode ,BirthDate) VALUES ( @firstName, @lastname, @nationalCode ,@birthDate)", cn);
      cmd.Parameters.AddWithValue("@firstName", request.FirstName);
      cmd.Parameters.AddWithValue("@lastName", request.LastName);
      cmd.Parameters.AddWithValue("@nationalCode", request.NationalCode);
      cmd.Parameters.AddWithValue("@birthDate", request.BirthDate);

      cn.Open();
      var i = cmd.ExecuteNonQuery();
      cn.Close();


      return Task.FromResult(new InsertReply { Id = i.ToString() });
    }

    public override Task<UpdateReply> Update(UpdateRequest request, ServerCallContext context)
    {
      var cn = new SqlConnection("Data Source=.;initial catalog=Test; User ID=sa;Password=1234;");

      var cmd = new SqlCommand("update dbo.MyTable set FirstName = @firstName, Lastname=  @lastname, NationalCode =@nationalCode ,BirthDate = @birthDate where Id = @id", cn);
      cmd.Parameters.AddWithValue("@id", request.Id);
      cmd.Parameters.AddWithValue("@firstName", request.FirstName);
      cmd.Parameters.AddWithValue("@lastName", request.LastName);
      cmd.Parameters.AddWithValue("@nationalCode", request.NationalCode);
      cmd.Parameters.AddWithValue("@birthDate", request.BirthDate);

      cn.Open();
      var i = cmd.ExecuteNonQuery();
      cn.Close();


      return Task.FromResult(new UpdateReply { Id = request.Id });
    }

    public override Task<DeleteReply> Delete(DeleteRequest request, ServerCallContext context)
    {
      var cn = new SqlConnection("Data Source=.;initial catalog=Test; User ID=sa;Password=1234;");

      var cmd = new SqlCommand("delete dbo.MyTable where Id = @id", cn);
      cmd.Parameters.AddWithValue("@id", request.Id);

      cn.Open();
      var i = cmd.ExecuteNonQuery();
      cn.Close();

      return Task.FromResult(new DeleteReply { Id = request.Id });
    }

    public override Task<GetReply> Get(GetRequest request, ServerCallContext context)
    {
      var cn = new SqlConnection("Data Source=.;initial catalog=Test; User ID=sa;Password=1234;");

      var cmd = new SqlCommand("select * from dbo.MyTable where Id = @id", cn);
      cmd.Parameters.AddWithValue("@id", request.Id);

      cn.Open();
      var reader = cmd.ExecuteReader();

      var dataTable = new DataTable();
      dataTable.Load(reader);

      var myObjects = new List<GetReply>();

      if (dataTable.Rows.Count > 0)
      {
        var serializedMyObjects = JsonConvert.SerializeObject(dataTable);
        myObjects = (List<GetReply>)JsonConvert.DeserializeObject(serializedMyObjects, typeof(List<GetReply>));
      }

      cn.Close();


      return Task.FromResult(myObjects.FirstOrDefault());
    }
  }
}