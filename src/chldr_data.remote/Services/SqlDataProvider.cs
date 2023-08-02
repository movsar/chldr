﻿using chldr_data.Interfaces;
using chldr_utils;
using chldr_utils.Services;
using chldr_data.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using Microsoft.Extensions.Hosting;

namespace chldr_data.remote.Services
{
    public class SqlDataProvider : IDataProvider
    {
        public bool IsInitialized { get; set; }
        public event Action? DatabaseInitialized;

        private readonly FileService _fileService;
        private DbContextOptions<SqlContext> _options;
        private readonly ExceptionHandler _exceptionHandler;

        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, IConfiguration configuration)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;

            var connectionString = configuration.GetConnectionString("SqlContext");
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new NullReferenceException("Connection string is empty");
            }

            _options = new DbContextOptionsBuilder<SqlContext>()
               .UseMySQL(connectionString)
               .Options;
        }
        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, string connectionString)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;

            _options = new DbContextOptionsBuilder<SqlContext>()
               .UseMySQL(connectionString)
               .Options;
        }

        public SqlDataProvider(FileService fileService, ExceptionHandler exceptionHandler, DbContextOptions<SqlContext> dbContextOptionsBuilder)
        {
            _fileService = fileService;
            _exceptionHandler = exceptionHandler;
            _options = dbContextOptionsBuilder;
        }

        public IUnitOfWork CreateUnitOfWork(string? userId = null)
        {
            return new SqlUnitOfWork(_options, _fileService, _exceptionHandler, userId);
        }

        public void Initialize()
        {
            DatabaseInitialized?.Invoke();
        }

        public void TruncateDatabase()
        {

        }

    }
}