using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using WebCoreTest5.Db;
using WebCoreTest5.Model;

namespace WebCoreTest5.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class TestFourController : BaseController
    {
        protected ApplicationDbContext _db = null;

        public TestFourController(ApplicationDbContext db)
        {
            this._db = db;
        }

        [HttpGet]
        public int AddUser()
        {
            int size = _db.users.Count();
            _db.users.Add(new userinfo() { user_name = "张三" + size });

            _db.SaveChanges();

            return 0;
        }

        [HttpGet]
        public int AddCard()
        {
            int size = _db.cards.Count();
            _db.cards.Add(new cardinfo() { card_num = size.ToString() + size.ToString(), addId = 1, openId = 2 });
            _db.SaveChanges();

            return 0;
        }

        [HttpGet]
        public ActionResult Test1()
        {
            // 由于存在循环引用的问题，所以这里处理，返回需要进行显示的内容，
            // 如果使用NetstoreJson，就配置 SeariazeOptions进行处理
            // 参考：https://blog.csdn.net/weixin_41587645/article/details/89607442
            var card = _db.cards.Include(c => c.open).Where(p => p.cardId == 1).Select(p => new { p.card_num, add_user = p.add.user_name, open_user = p.open.user_name }).FirstOrDefault();
            if (card != null)
            {
                return new JsonResult(card);
            }
            else
            {
                return new JsonResult(new cardinfo());
            }
        }

        [HttpGet]
        public ActionResult Test2()
        {
            var card = _db.cards.Include(c => c.open).Where(p => p.cardId == 1).FirstOrDefault();
            if (card != null)
            {
                return new JsonResult(card);
            }
            else
            {
                return new JsonResult(new cardinfo());
            }
        }

        [HttpGet]
        public ActionResult Test3()
        {
            var card = _db.cards2.Include(c => c.open).Where(p => p.id == 1).FirstOrDefault();
            if (card != null)
            {
                return new JsonResult(card);
            }
            else
            {
                return new JsonResult(new cardinfo());
            }
        }
    }
}
