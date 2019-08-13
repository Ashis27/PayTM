using Kare4uPaymentPlatform.Models.PaymentGateway;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;

namespace Kare4uPaymentPlatform.Models
{
	public class Care4UContext : DbContext, IUnitOfWork
	{
		////Payment Gateway Related
		public DbSet<InterimOrderDeatils> InterimOrderDeatils { get; set; }
		public DbSet<PaymentGatewayTransactions> PaymentGatewayTransactions { get; set; }
		public DbSet<Care4UPgConfig> Care4uPgConfig { get; set; }
		public DbSet<PGTransactionBreakup> PGTransactionBreakup { get; set; }

		protected override void OnModelCreating(DbModelBuilder modelBuilder)
		{
			
		}


		public new void SaveChanges()
		{
			//For audit fields
			ObjectContext context = ((IObjectContextAdapter)this).ObjectContext;
			//Find all Entities that are Added/Modified that inherit from my EntityBase
			IEnumerable<ObjectStateEntry> objectStateEntries =
			from e in context.ObjectStateManager.GetObjectStateEntries(EntityState.Added | EntityState.Modified)
			where
			e.IsRelationship == false &&
			e.Entity != null
			select e;

			var currentTime = DateTime.Now;

			foreach (var entry in objectStateEntries)
			{
				var entityBase = entry.Entity;
				CurrentValueRecord entryValues = entry.CurrentValues;

				//Only do if the entities have the audit columns (to be on the safer side)
				int ordinalValue = 0;
				try
				{
					ordinalValue = entryValues.GetOrdinal("CreatedDate");
				}
				catch (Exception)
				{
					ordinalValue = 0;
				}
				if (ordinalValue > 0)
				{
					//Getting the user ID
					string userID = "9999";// string.Empty;
					//try
					//{
					//	userID = HelperService.ContextObject.UserID.ToString();
					//}
					//catch (Exception)
					//{

					//	userID = "9999";
					//}
					//string userID = Convert.ToString(HelperService.ContextObject.UserID);
					//string userID = HelperService.ContextObject.UserID.ToString();
					//Irrespective of update or insert always the updated columns are updated
					//To be doubly sure, put try catch
					try
					{
						entryValues.SetDateTime(entryValues.GetOrdinal("UpdatedDate"), DateTime.Now);
						entryValues.SetString(entryValues.GetOrdinal("UpdatedBy"), userID);
						//If insert, update the other two audit columns as well
						if (entry.State == EntityState.Added)
						{
							entryValues.SetDateTime(entryValues.GetOrdinal("CreatedDate"), DateTime.Now);
							entryValues.SetString(entryValues.GetOrdinal("CreatedBy"), userID);
						}
						// as there is no deletion so we assume that entity state can only be modified -- siddharth (as discussed with brahma on 24th Jan 2014)
						else
						{
							var dbValueOfEntity = this.Entry(entry.Entity).GetDatabaseValues();
							//var createdDate = this.Entry(entry.Entity).GetDatabaseValues().GetValue<DateTime?>("CreatedDate");
							var createdDate = dbValueOfEntity.GetValue<DateTime?>("CreatedDate");
							var createdBy = dbValueOfEntity.GetValue<string>("CreatedBy");
							if (createdDate != null)
							{
								entryValues.SetDateTime(entryValues.GetOrdinal("CreatedDate"), createdDate.Value);
							}
							if (createdBy != null)
							{
								var createdByOrdinal = entryValues.GetOrdinal("CreatedBy");
								//if (createdByOrdinal.GetType() == typeof(string))
								//{
								// entryValues.SetString(createdByOrdinal, createdBy);
								//}
								//else // the value createdby is of integer type
								//{
								// entryValues.SetInt32(createdByOrdinal, Convert.ToInt32(createdBy));
								//}
								entryValues.SetString(createdByOrdinal, createdBy);

							}
						}

					}
					catch (Exception)
					{
						//Do nothing
					}

				}

			}
			//End audit fields
			base.SaveChanges();
		}




		public void Attach<T>(T obj) where T : class
		{
			//Set<T>().Attach(obj);
			throw new NotImplementedException();
		}

		public void Add<T>(T obj) where T : class
		{
			Set<T>().Add(obj);
		}

		public IQueryable<T> Get<T>() where T : class
		{
			return Set<T>();
		}

		public bool Remove<T>(T item) where T : class
		{
			try
			{
				Set<T>().Remove(item);

			}
			catch (Exception)
			{
				return false;
			}
			return true;
		}

		protected IUnitOfWork UnitOfWork { get; set; }
		protected Care4UContext Context
		{
			get { return (Care4UContext)this.UnitOfWork; }
		}
	}

	public interface IUnitOfWork
	{
		void SaveChanges();
		void Attach<T>(T obj) where T : class;
		void Add<T>(T obj) where T : class;
		IQueryable<T> Get<T>() where T : class;
		bool Remove<T>(T item) where T : class;
	}
}