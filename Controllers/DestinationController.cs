﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZTourist.Infrastructure;
using ZTourist.Models;
using ZTourist.Models.ViewModels;

namespace ZTourist.Controllers
{
    [Authorize(Policy = "NotEmployee")]
    public class DestinationController : Controller
    {
        private readonly DestinationDAL destinationDAL;
        private readonly ILogger logger;

        public DestinationController(DestinationDAL destinationDAL, ILogger logger)
        {
            this.destinationDAL = destinationDAL;
            this.logger = logger;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            try
            {
                DestinationSearchViewModel model = new DestinationSearchViewModel { IsActive = true };
                model.Skip = (page - 1) * model.Fetch;
                int total = await destinationDAL.GetTotalDestinationsAsync(model.IsActive);
                PageInfo pageInfo = new PageInfo
                {
                    TotalItems = total,
                    ItemPerPage = model.Fetch,
                    PageAction = nameof(Index),
                    CurrentPage = page
                };
                model.Destinations = await destinationDAL.GetAllDestinationsAsync(model.IsActive, model.Skip, model.Fetch);
                model.PageInfo = pageInfo;
                ViewBag.Title = "All Destinations";
                return View(model);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }            
        }

        public async Task<IActionResult> Details(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return NotFound();
                Destination destination = await destinationDAL.FindDestinationByIdAsync(id, true);
                if (destination == null)
                {
                    return NotFound();
                }
                return View(destination);
            }
            catch (Exception ex)
            {
                logger.LogError(ex.Message);
                throw;
            }            
        }

    }
}
