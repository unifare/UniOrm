using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OauthMngPlugin.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using UniOrm;
using UniOrm.Common;

using Microsoft.Extensions.DependencyInjection;
namespace OauthMngPlugin.Contorllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserMngController : ControllerBase
    {
        readonly OauthMngModule OauthMngModule;
        public UserMngController(OauthMngModule oauthMngModule)
        {
           // dd = OauthMngModule.ServiceProvider.GetService<Itest>();
            OauthMngModule = oauthMngModule;
        }

        [AdminAuthorize]
        [HttpGet]
        public async Task<IEnumerable<OauthUser>> GetAllUser()
        {
            string wherestring = ""; int pindex = 0; int pagesize = 10;
            var query = OauthMngModule.DB.Client.Queryable<OauthUser>();
            query.WhereIF(!string.IsNullOrEmpty(wherestring),
                it => it.Name.Contains(wherestring) || it.UserName.Contains(wherestring));
          
            return await query.ToPageListAsync(pindex, pagesize);
        }


        //[AdminAuthorize]
        //[HttpGet]
        //public async Task<IEnumerable<OauthUser>> GetAllUser()
        //{
        //    string wherestring = ""; int pindex = 0; int pagesize = 10;
        //    var query = OauthMngModule.DB.Client.Queryable<OauthUser>();
        //    query.WhereIF(!string.IsNullOrEmpty(wherestring),
        //        it => it.Name.Contains(wherestring) || it.UserName.Contains(wherestring));

        //    return await query.ToPageListAsync(pindex, pagesize);
        //}
    }
}
