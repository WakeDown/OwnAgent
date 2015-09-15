using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using OwnAgent.Objects;

namespace OwnAgent.Models
{
    public class DbInit
    {
        string ClientId { get; set; }
        public DbInit(string clientId)
        {
            ClientId = clientId;
        }

        public DbInit(string clientId, string login):this(clientId)
        {
            var db = new BalanceContext();
            if (!db.Clients.Any(x => x.ClientId.Equals(clientId)))
            {
                db.Clients.Add(new Client(clientId, login));
                db.SaveChanges();
            }
        }
        

    public void InitBalance()
        {
            var context = new BalanceContext();

            if (!context.SpendCategories.Any(x => x.ClientId.Equals(ClientId)))
            {
                var spendCats = new List<SpendCategory>();
                spendCats.Add(new SpendCategory("Еда/Бытовое", 1, ClientId) { Selected = true });
                spendCats.Add(new SpendCategory("Ресторан", 2, ClientId));
                spendCats.Add(new SpendCategory("Бензин", 3, ClientId));
                spendCats.Add(new SpendCategory("Услуги", 4, ClientId));
                spendCats.Add(new SpendCategory("Одежда", 5, ClientId));
                spendCats.Add(new SpendCategory("Зарплата", 6, ClientId));
                spendCats.Add(new SpendCategory("Подработка", 7, ClientId));
                spendCats.Add(new SpendCategory("Вклад", 8, ClientId));
                spendCats.Add(new SpendCategory("Другое", 9, ClientId));
                spendCats.ForEach(s => context.SpendCategories.Add(s));
                context.SaveChanges();
            }

            if (!context.SpendVectors.Any(x => x.ClientId.Equals(ClientId)))
            {
                var spendVectors = new List<SpendVector>();
                spendVectors.Add(new SpendVector("расход", 1, ClientId) { Selected = true });
                spendVectors.Add(new SpendVector("доход", 2, ClientId));
                spendVectors.Add(new SpendVector("инвест", 3, ClientId));
                spendVectors.ForEach(s => context.SpendVectors.Add(s));
                context.SaveChanges();
            }
        }
    }
}