using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBWork
{
    public class DBFactory//паттерн фабрика
    {
        /// <summary>
        /// Создание базы данных из фабрики
        /// </summary>
        /// <param name="type">Тип БД: "sqlite" - SQLite</param>
        /// <param name="route">Путь до БД</param>
        /// <returns>Указатель на БД</returns>
        /// <exception cref="ArgumentException">Тип БД был указан неверно</exception>
        public static DataBase CreateDatabase(string type, string route)
        {
            DataBase dataBase = null;
            switch (type.ToLower())
            {
                case "sqlite":
                    dataBase = new SQLiteDB(route);
                    break;
                default:
                    throw new ArgumentException("Invalid database type");
            }
            return dataBase;
        }
    }
}
