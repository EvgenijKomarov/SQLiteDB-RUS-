using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SQLite;

namespace DBWork
{
    public delegate void HandleError(string message);
    public abstract class DataBase
    {
        protected string _route;//Путь до БД
        protected readonly SemaphoreSlim _lockConnection = new(1, 1);//Защита соединения. Только одна функция может иметь доступ к БД
        /// <summary>
        /// Выполнение транзакции асинхронно
        /// </summary>
        /// <param name="queries">Список команд для транзакции</param>
        /// <returns>Результат выполнения</returns>
        public abstract Task<bool> SendTransactionAsync(List<string> queries);
        /// <summary>
        /// Выполнение транзакции синхронно
        /// </summary>
        /// <param name="queries">Список команд для транзакции</param>
        /// <returns>Результат выполнения</returns>
        public abstract bool SendTransaction(List<string> queries);
        /// <summary>
        /// Отправка команды без ответа асинхронно
        /// </summary>
        /// <param name="command">Команда</param>
        /// <param name="variablesData">Обработка команды перед её выполнением. Возможно необходимо для использования переменных. 
        /// По умолчанию оставить null </param>
        /// <returns>Возвращает количество изменённых сторок</returns>
        public abstract Task<int> SendNonQueryAsync(string command, Action<SQLiteCommand> variablesData = null);
        /// <summary>
        /// Отправка команды с получением ответа асинхронно
        /// </summary>
        /// <param name="query">Команда</param>
        /// <returns>Возвращает объект DataTable в качестве ответа от БД</returns>
        public abstract Task<DataTable> SendQueryAsync(string query);
        /// <summary>
        /// Отправка команды без ответа синхронно
        /// </summary>
        /// <param name="command">Команда</param>
        /// <param name="variablesData">Обработка команды перед её выполнением. Возможно необходимо для использования переменных. 
        /// По умолчанию оставить null </param>
        /// <returns>Возвращает количество изменённых сторок</returns>
        public abstract int SendNonQuery(string command, Action<SQLiteCommand> variablesData = null);
        /// <summary>
        /// Отправка команды с получением ответа синхронно
        /// </summary>
        /// <param name="query">Команда</param>
        /// <returns>Возвращает объект DataTable в качестве ответа от БД</returns>
        public abstract DataTable SendQuery(string query);
    }
}
