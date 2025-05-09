﻿using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using SportsStore.Infrastructure;

namespace SportsStore.Models
{
    public class SessionCart : Cart
    {
        [JsonIgnore]
        public ISession? Session { get; set; }

        public static Cart GetCart(IServiceProvider services)
        {
            ISession? session = services.GetRequiredService<IHttpContextAccessor>()?
                .HttpContext?.Session;

            SessionCart cart = session?.GetJson<SessionCart>("Cart") ?? new SessionCart();
            cart.Session = session;
            return cart;
        }

        public override void AddItem(Product product, int quantity)
        {
            if (product == null) throw new ArgumentNullException(nameof(product));
            base.AddItem(product, quantity);
            Session?.SetJson("Cart", this);
        }

        public override void RemoveLine(int productId)
        {
            base.RemoveLine(productId);
            Session?.SetJson("Cart", this);
        }

        public override void Clear()
        {
            base.Clear();
            Session?.Remove("Cart");
        }
    }
}