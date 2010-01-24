using System;
using System.Collections.Generic;
using System.Linq;
using MvcBookApplication.Data.Models;

namespace MvcBookApplication.Data
{
    public class InMemoryMessageRepository : IMessageRepository
    {
        private List<Message> Messages { get; set; }

        public InMemoryMessageRepository()
        {
            Messages = new List<Message>();
        }

        private int _autoId;
        private int AutoId
        {
            get
            {
                _autoId += 1;
                return _autoId;
            }
        }

        public int Add(Message message)
        {
            message.Id = AutoId;
            Messages.Add(message);
            return message.Id;
        }


        public bool Delete(int Id)
        {
            try
            {
                Messages.Remove(Messages.Single(m => m.Id == Id));
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool Save(Message message)
        {
            try
            {
                var original = Messages.Single(m => m.Id == message.Id);
                var index = Messages.IndexOf(original);
                message.User = original.User;
                Messages[index] = message;
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IQueryable<Message> Get()
        {
            var q = from m in Messages select m;
            return q.AsQueryable();
        }
    }
}