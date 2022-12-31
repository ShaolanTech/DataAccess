using System.Data.Common;

namespace ShaolanTech.Data
{
    public class TransactionWrapper
    {
        public DbConnection Connection { get; set; }
        public DbTransaction Transaction { get; set; }
        public string Message { get; set; }
        public bool OperationDone { get; set; }
        public void Commit()
        {
            this.Transaction.Commit();
            this.Transaction.Dispose();
            this.Connection.Close();
        }
        public void Rollback()
        {
            this.Transaction.Rollback();
            this.Transaction.Dispose();
            this.Connection.Close();
        }
    }
}
