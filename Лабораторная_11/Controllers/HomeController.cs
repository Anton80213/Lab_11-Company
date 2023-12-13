using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Лабораторная_11.Models;
using System.Windows.Forms;
using System.Data;
using Newtonsoft.Json;

namespace Лабораторная_11.Controllers
{
    public class HomeController : Controller
    {
        // создаем контекст данных
        static EmployeeContext db = new EmployeeContext();
        public string Home()
        {
            return null;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Переход(string Логин, string Пароль)
        {
            if (Логин == "AdMin546729" && Пароль == "lobster2091" )
            {
                MessageBox.Show("Данные верны");
                return RedirectToAction("Домашняя");
            }
            else
            {
                MessageBox.Show("Неверный логин или пароль");
                return RedirectToAction("Index");
            }
        }
        [HttpGet]
        public ActionResult Домашняя()
        {
            Model_College model = new Model_College()
            {
                сотрудники = db.Сотрудники.ToList(),
                сотрудники2 = db.Сотрудники2.ToList(),
                должности = db.Должности.ToList()
            };

            return View(model);
        }
        [HttpGet]
        public ActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create(Сотрудник сотрудник)
        {
            var Запрос = from x in db.Сотрудники
                         where x.Шифр == сотрудник.Шифр
                         group x by x.Шифр into g
                         select g.Count();
            foreach (var i in Запрос)
            {
                if (i > 0)
                {
                    MessageBox.Show("Ошибка: сотрудник с данным шифром уже существует");
                    return RedirectToAction("Create");
                }
            }
            db.Сотрудники.Add(сотрудник);
            db.SaveChanges();
            return RedirectToAction("Домашняя");
        }
        [HttpGet]
        public ActionResult Create2()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create2(Сотрудник2 сотрудник2)
        {
            var Запрос2 = from x in db.Сотрудники2
                         where x.Шифр == сотрудник2.Шифр 
                         group x by x.Шифр into g
                         select g.Count();
            foreach (var i in Запрос2)
            {
                if (i > 2) MessageBox.Show("Трудовой опыт данного сотрудника - более 2 специальностей");
            }
            db.Сотрудники2.Add(сотрудник2);
            db.SaveChanges();
            return RedirectToAction("Домашняя");
        }
        [HttpGet]
        public ActionResult Create3()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Create3(Должность должность)
        {
            db.Должности.Add(должность);
            db.SaveChanges();
            return RedirectToAction("Домашняя");
        }
        [HttpGet]
        public ActionResult Выборка_данных1()
        {
            var query = from x in db.Сотрудники2.AsEnumerable()
                        where x.Должность == "Инженер"
                        group x by x.Шифр into g
                        join y in db.Сотрудники.AsEnumerable()
                        on g.FirstOrDefault().Шифр equals y.Шифр
                        select y; // запрос, выводящий информацию о сотруднике из таблицы "Сотрудники", который работал (работает) по специальности "Инженер"
            return View(query);
        }
        [HttpGet]
        public ActionResult Выборка_данных2()
        {
            var query = from x in db.Сотрудники2.AsEnumerable()
                        where x.Шифр == 1 && x.Дата_увольнения == "-"
                        select new Model1{ Шифр = x.Шифр, Должность = x.Должность, Зарплата = x.Зарплата, Дата_принятия = x.Дата_принятия };
            // запрос, выводящий шифр, текущую должность, текущую зарплату и дату принятия на текущую работу сотрудника из таблицы "Сотрудники2", который имеет шифр, равный 1, и работает
            return View(query);
        }
        [HttpGet]
        public ActionResult Выборка_данных3()
        {
            DateTime date1 = new DateTime(1990, 01, 01);
            DateTime date2 = new DateTime(1995, 01, 01);
            var query = from x in db.Сотрудники2.AsEnumerable()
                        where DateTime.Compare(date1, DateTime.Parse(x.Дата_принятия)) < 0 && DateTime.Compare(date2, DateTime.Parse(x.Дата_принятия)) > 0
                        group x by x.Шифр into g
                        join y in db.Сотрудники.AsEnumerable()
                        on g.FirstOrDefault().Шифр equals y.Шифр
                        select y; // запрос, выводящий информацию о сотруднике из таблицы "Сотрудники", который был принят на работу в промежутке между 01.01.1990 и 01.01.1995 г.
            return View(query);
        }
        [HttpGet]
        public ActionResult Выборка_данных4()
        {
            var Список = (from x in db.Сотрудники2.AsEnumerable()
                        group x by x.Должность into g
                        select new Model2{ Должность = g.Key, Максимальная_зарплата = g.Max(x => x.Зарплата), Минимальная_зарплата = g.Min(x => x.Зарплата) }).ToList();
            var Должности = from x in db.Сотрудники2.AsEnumerable()
                            select x.Должность;
            var query = Должности.Union(from y in db.Должности.AsEnumerable() select y.Название);
            foreach (var item in query)
            {
               if (Должности.Contains(item) == false)
               { 
                    var a = new Model2{ Должность = item, Максимальная_зарплата = 0, Минимальная_зарплата = 0 };
                    Список.Add(a);
               }
            }
            var result = Список.AsEnumerable(); // результат - должность сотрудника, максимальная и минимальная зарплата по этой должности
            return View(result);
        }
        
    }
}