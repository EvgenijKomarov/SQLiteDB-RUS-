//using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System;
using System.Data;
using System.Data.SQLite;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBWork
{
    public class SQLiteDB : DataBase
    {
        public SQLiteDB(string connect_route)
        {
            try
            {
                using (var db = new SQLiteConnection($"Data Source={connect_route}"))
                {
                    db.Open();
                    db.Close();
                }
                _route = connect_route;
            }
            catch (Exception ex)
            {
                _route = "";
            }
        }
        public override async Task<bool> SendTransactionAsync(List<string> queries)
        {
            bool success = false;  // Переменная для хранения результата выполнения
            await _lockConnection.WaitAsync();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_route}"))
            {
                await connection.OpenAsync();  // Открытие соединения асинхронно

                using (SQLiteTransaction transaction = (SQLiteTransaction)await connection.BeginTransactionAsync())  // Начало транзакции асинхронно
                {
                    try
                    {
                        // Выполнение SQL-запросов внутри транзакции
                        foreach (string query in queries)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = query;
                                await command.ExecuteNonQueryAsync();  // Асинхронное выполнение команды
                            }
                        }

                        // Если все операции прошли успешно, фиксируем транзакцию
                        await transaction.CommitAsync();  // Фиксирование транзакции асинхронно
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        // Если произошла ошибка, откатываем транзакцию
                        await transaction.RollbackAsync();  // Откат транзакции асинхронно
                        success = false;
                    }
                }  // Транзакция автоматически завершается при выходе из блока using
            }  // Соединение закрывается автоматически при выходе из блока using

            _lockConnection.Release();
            return success;
        }
        public override bool SendTransaction(List<string> queries)
        {
            bool success = false;  // Переменная для хранения результата выполнения
            _lockConnection.Wait();
            using (SQLiteConnection connection = new SQLiteConnection($"Data Source={_route}"))
            {
                connection.Open();  // Открытие соединения

                using (SQLiteTransaction transaction = connection.BeginTransaction())  // Начало транзакции
                {
                    try
                    {
                        // Выполнение SQL-запросов внутри транзакции
                        foreach (string query in queries)
                        {
                            using (SQLiteCommand command = connection.CreateCommand())
                            {
                                command.Transaction = transaction;
                                command.CommandText = query;
                                command.ExecuteNonQuery();  // Выполнение команды
                            }
                        }

                        // Если все операции прошли успешно, фиксируем транзакцию
                        transaction.Commit();  // Фиксирование транзакции 
                        success = true;
                    }
                    catch (Exception ex)
                    {
                        // Если произошла ошибка, откатываем транзакцию
                        transaction.Rollback();  // Откат транзакции
                        success = false;
                    }
                }  // Транзакция автоматически завершается при выходе из блока using
            }  // Соединение закрывается автоматически при выходе из блока using
            _lockConnection.Release();
            return success;
        }
        public override async Task<int> SendNonQueryAsync(string command, Action<SQLiteCommand> variablesData = null)
        {
            int rowsAffected = 0;
            await _lockConnection.WaitAsync();
            using (var connection = new SQLiteConnection($"Data Source={_route}"))
            {
                await connection.OpenAsync();  // Открываем соединение асинхронно

                using (var SQLcommand = new SQLiteCommand(command, connection))
                {
                    variablesData?.Invoke(SQLcommand);  // Добавляем параметры к команде, если они есть
                    rowsAffected = await SQLcommand.ExecuteNonQueryAsync();  // Выполняем команду асинхронно
                }
            }
            _lockConnection.Release();
            return rowsAffected;
        }
        public override async Task<DataTable> SendQueryAsync(string query)
        {
            DataTable dataTable = new DataTable();
            await _lockConnection.WaitAsync();
            using (var connection = new SQLiteConnection($"Data Source={_route}"))
            {
                await connection.OpenAsync();

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            _lockConnection.Release();
            return dataTable;
        }
        public override int SendNonQuery(string command, Action<SQLiteCommand> variablesData = null)
        {
            int rowsAffected = 0;
            _lockConnection.Wait();
            using (var connection = new SQLiteConnection($"Data Source={_route}"))
            {
                connection.Open();  // Открываем соединение асинхронно

                using (var SQLcommand = new SQLiteCommand(command, connection))
                {
                    variablesData?.Invoke(SQLcommand);  // Добавляем параметры к команде, если они есть
                    rowsAffected = SQLcommand.ExecuteNonQuery();  // Выполняем команду асинхронно
                }
            }
            _lockConnection.Release();
            return rowsAffected;
        }
        public override DataTable SendQuery(string query)
        {
            DataTable dataTable = new DataTable();
            _lockConnection.Wait();
            using (var connection = new SQLiteConnection($"Data Source={_route}"))
            {
                connection.Open();

                using (var command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataAdapter adapter = new SQLiteDataAdapter(command))
                    {
                        adapter.Fill(dataTable);
                    }
                }
            }
            _lockConnection.Release();
            return dataTable;
        }
    }
}
