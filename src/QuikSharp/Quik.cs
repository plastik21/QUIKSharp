// Copyright (c) 2014-2020 QUIKSharp Authors https://github.com/finsight/QUIKSharp/blob/master/AUTHORS.md. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE.txt in the project root for license information.

using System;

namespace QuikSharp
{
    /// <summary>
    /// Quik interface in .NET
    /// </summary>
    public sealed class Quik
    {
        /// <summary>
        /// 34130
        /// </summary>
        public const int DefaultPort = 34130;

        /// <summary>
        /// localhost
        /// </summary>
        public const string DefaultHost = "127.0.0.1";

        /// <summary>
        /// Default timeout to use for send operations if no specific timeout supplied.
        /// </summary>
        public TimeSpan DefaultSendTimeout
        {
            get => QuikService.DefaultSendTimeout;
            set => QuikService.DefaultSendTimeout = value;
        }

        public bool IsServiceConnected => QuikService.IsServiceConnected();

        /// <summary>
        /// Quik current data is all in local time. This property allows to convert it to UTC datetime
        /// </summary>
        public TimeZoneInfo TimeZoneInfo { get; set; }

        /// <summary>
        /// Persistent transaction storage
        /// </summary>
        public IPersistentStorage Storage { get; set; }

        /// <summary>
        /// Debug functions
        /// </summary>
        public DebugFunctions Debug { get; set; }

        /// <summary>
        /// Функции обратного вызова
        /// </summary>
        public IQuikEvents Events { get; set; }

        /// <summary>
        /// Сервисные функции
        /// </summary>
        public IServiceFunctions Service { get; private set; }

        /// <summary>
        /// Функции для обращения к спискам доступных параметров
        /// </summary>
        public IClassFunctions Class { get; private set; }

        /// <summary>
        /// Функции для работы со стаканом котировок
        /// </summary>
        public IOrderBookFunctions OrderBook { get; set; }

        /// <summary>
        /// Функции взаимодействия скрипта Lua и Рабочего места QUIK
        /// </summary>
        public ITradingFunctions Trading { get; set; }

        /// <summary>
        /// Функции для работы со стоп-заявками
        /// </summary>
        public StopOrderFunctions StopOrders { get; private set; }

        /// <summary>
        /// Функции для работы с заявками
        /// </summary>
        public OrderFunctions Orders { get; private set; }

        /// <summary>
        /// Функции для работы со свечами
        /// </summary>
        public CandleFunctions Candles { get; private set; }

        /// <summary>
        /// Сервис
        /// </summary>
        private QuikService QuikService { get; set; }

        /// <summary>
        /// Quik interface in .NET constructor
        /// </summary>
        public Quik(int port = DefaultPort, IPersistentStorage storage = null, string host = DefaultHost)
        {
            Storage = storage ?? new InMemoryStorage();

            QuikService = QuikService.Create(port, host);
            QuikService.Storage = Storage;
            QuikService.Candles = new CandleFunctions(port, host);
            QuikService.StopOrders = new StopOrderFunctions(port, this, host);

            Events = QuikService.Events;
            Candles = QuikService.Candles;
            StopOrders = QuikService.StopOrders;
            Debug = new DebugFunctions(port, host);
            Service = new ServiceFunctions(port, host);
            Class = new ClassFunctions(port, host);
            OrderBook = new OrderBookFunctions(port, host);
            Trading = new TradingFunctions(port, host);
            Orders = new OrderFunctions(port, this, host);

            Events.OnConnectedToQuik += (_) =>
            {
                QuikService.WorkingFolder = Service.GetWorkingFolder().Result;
            };
        }

        public void Start()
        {
            QuikService.Start();
        }

        public void Stop()
        {
            QuikService.Stop();
        }
    }
}