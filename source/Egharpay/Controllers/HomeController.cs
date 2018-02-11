﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Routing;
using Configuration.Interface;
using Egharpay.Business.Extensions;
using Egharpay.Business.Interfaces;
using Egharpay.Entity.Dto;
using Egharpay.Extensions;
using Egharpay.Models;
using Microsoft.Owin.Security.Authorization;
using Egharpay.Business.Enum;

namespace Egharpay.Controllers
{
    // [Authorize]
    public class HomeController : BaseController
    {
        private readonly IYouTubeBusinessService _youTubeBusinessService;
        private readonly IMobileBusinessService _mobileBusinessService;
        private readonly IPersonnelBusinessService _personnelBusinessService;
        private readonly ISellerBusinessService _sellerBusinessService;
        public HomeController(IMobileBusinessService mobileBusinessService, IYouTubeBusinessService youTubeBusinessService, IConfigurationManager configurationManager, IAuthorizationService authorizationService, IPersonnelBusinessService personnelBusinessService, ISellerBusinessService sellerBusinessService) : base(configurationManager, authorizationService)
        {
            _youTubeBusinessService = youTubeBusinessService;
            _personnelBusinessService = personnelBusinessService;
            _mobileBusinessService = mobileBusinessService;
            _sellerBusinessService = sellerBusinessService;
        }

        public async Task<ActionResult> Index()
        {
            var viewModel = new HomeViewModel();
            if (User.Identity.IsAuthenticated && !User.IsSuperUserOrAdmin())
                viewModel.PersonnelId = UserPersonnelId;
            if (User.Identity.IsAuthenticated && User.IsSeller())
            {
                var seller = await _sellerBusinessService.RetrieveSellerByPersonnelId(viewModel.PersonnelId);
                if (seller == null)
                    return RedirectToAction("Login", "Account");
                viewModel.IsSellerApproved = seller.ApprovalStateId == (int)ApprovalState.Approved;
            }
            return View(viewModel);
        }

        [AllowAnonymous]
        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View(new BaseViewModel());
        }

        [AllowAnonymous]
        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View(new BaseViewModel());
        }

        [HttpPost]
        public ActionResult GetCentres()
        {
            return this.JsonNet(null);
        }

        [HttpPost]
        public ActionResult Search()
        {
            return this.JsonNet(null);
        }

        //[HttpPost]
        [Route("Home/Mobile/{searchKeyword}")]
        public ActionResult Mobile(string searchKeyword)
        {
            var viewModel = new HomeViewModel()
            {
                SearchKeyword = searchKeyword
            };
            return View(viewModel);
        }

        [HttpPost]
        public async Task<ActionResult> SearchMobile(string searchKeyword, Paging paging, List<OrderBy> orderBy)
        {
            var data = await _mobileBusinessService.Search(searchKeyword, orderBy, paging);
            return this.JsonNet(data);
        }

        [HttpPost]
        public async Task<ActionResult> LatestMobile(bool showAll)
        {
            var data = await _mobileBusinessService.RetrieveLatestMobile();
            return this.JsonNet(showAll ? data : data.Take(5));
        }

        //[AllowAnonymous]
        public async Task<ActionResult> LatestMobileAll()
        {
            var mobileData = await _mobileBusinessService.RetrieveLatestMobile();
            return View(new BaseViewModel());
        }

        [HttpPost]
        public async Task<ActionResult> LatestMobileData()
        {
            var data = await _mobileBusinessService.RetrieveLatestMobile();
            return this.JsonNet(data);
        }

    }
}