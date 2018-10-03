using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MachineCalculator.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using UserDBWebRest.Resources;

namespace UserDBWebRest.Business
{
    public class PostgrsqlRepository : IRepository<AppObj>
    {
        public ServerCapacityContext _context;
        private ILogger _logger;

        public PostgrsqlRepository(ServerCapacityContext context)
        {
            _context = context;
            //_logger = logger;
        }

        public int Create(AppObj entity)
        {
            try
            {
                _context.AppObjDbSet.Add(entity);
                _context.SaveChanges();
               /// _logger.LogInformation(LoggingEvents.GenerateItems, "App added: ID: {ID}, Login: {LOGIN}, FirstName: {FIRSTNAME}, LastName: {LASTNAME}, MiddleName: {MIDDLENAME}, Password: {PASSWORD}", 
                ///    entity.Id 
                ///    );
                return entity.Id;
            }
            catch (Exception ex)
            {
                //_logger.LogError(LoggingEvents.GenerateItems, ex, "Error adding user ID:{ID}, Login: {LOGIN}, FirstName: {FIRSTNAME}, LastName: {LASTNAME}, MiddleName: {MIDDLENAME}, Password: {PASSWORD}", 
                 //   entity.Id 
                  //  );
                throw;
            }
        }

        public void Delete(int id)
        {
            try
            {
                var entity = GetById(id);
                if (entity == null)
                {
                    _logger.LogInformation(LoggingEvents.DeleteItemNotFound, "App id: {ID}", entity.Id);
                    throw new KeyNotFoundException();
                }
                else
                {
                    _context.AppObjDbSet.Remove(entity);
                    _context.SaveChanges();
                    _logger.LogInformation(LoggingEvents.DeleteItem, "App deleted: ID: {ID}, Login: {LOGIN}, FirstName: {FIRSTNAME}, LastName: {LASTNAME}, MiddleName: {MIDDLENAME}, Password: {PASSWORD}", 
                        entity.Id 
                        );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.DeleteItem, ex, "Error deleting user ID:{ID}", id);
                throw;
            }
        }

        public IEnumerable<AppObj> GetAll()
        {
            return _context.AppObjDbSet;
        }

        public AppObj GetById(int id)
        {
            try
            {
                return _context.AppObjDbSet.SingleOrDefault(e => e.Id == id);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void Update(AppObj entity)
        {
            try
            {
                if (GetById(entity.Id) != null)
                {
                    _context.Update(entity);
                    _context.SaveChanges();
                    _logger.LogInformation(LoggingEvents.UpdateItem, "App updated: ID: {ID}, Login: {LOGIN}, FirstName: {FIRSTNAME}, LastName: {LASTNAME}, MiddleName: {MIDDLENAME}, Password: {PASSWORD}",
                        entity.Id
                        );
                }
                else
                {
                    _logger.LogInformation(LoggingEvents.UpdateItemNotFound, "App id: {ID}", entity.Id);
                    throw new KeyNotFoundException();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(LoggingEvents.UpdateItem, ex, "Error updating user ID: {ID}, Login: {LOGIN}, FirstName: {FIRSTNAME}, LastName: {LASTNAME}, MiddleName: {MIDDLENAME}, Password: {PASSWORD}", 
                    entity.Id 
                    );
                throw;
            }
        }
    }
}
