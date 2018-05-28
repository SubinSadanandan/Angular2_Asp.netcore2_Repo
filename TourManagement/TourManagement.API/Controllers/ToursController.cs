using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TourManagement.API.Dtos;
using TourManagement.API.Helpers;
using TourManagement.API.Services;

namespace TourManagement.API.Controllers
{
    [Route("api/tours")]
    public class ToursController : Controller
    {
        private readonly ITourManagementRepository _tourManagementRepository;

        public ToursController(ITourManagementRepository tourManagementRepository)
        {
            _tourManagementRepository = tourManagementRepository;
        }
        
        [HttpGet]
        public async Task<IActionResult> GetTours()
        {
            var toursFromRepo = await _tourManagementRepository.GetTours();

            var tours = Mapper.Map<IEnumerable<Tour>>(toursFromRepo);
            return Ok(tours);
        }


        //[HttpGet("{tourId}", Name = "GetTour")]
        //public async Task<IActionResult> GetTour(Guid tourId)
        //{
        //    var tourFromRepo = await _tourManagementRepository.GetTour(tourId);

        //    if (tourFromRepo == null)
        //    {
        //        return BadRequest();
        //    }

        //    var tour = Mapper.Map<Tour>(tourFromRepo);

        //    return Ok(tour);
        //}    
        

        [HttpGet("{tourId}")]
        public async Task<IActionResult> GetDefaultTour(Guid tourId)
        {
            return await GetSpecificTour<Tour>(tourId);
        }


        [HttpGet("{tourId}",Name = "GetTour")]
        [RequestHeaderMatchesMediaType("Accept",new[] { "application/vnd.marvin.tour+json"})]
        public async Task<IActionResult> GetTour(Guid tourId)
        {
            return await GetSpecificTour<Tour>(tourId);
        }

        [HttpGet("{tourId}")]
        [RequestHeaderMatchesMediaType("Accept",new[] { "application/vnd.marvin.tourwithestimatedprofits+json" })]
        public async Task<IActionResult> GetTourWithEstimatedProfits(Guid tourId)
        {
            return await GetSpecificTour<TourWithEstimatedProfits>(tourId);
        }

        [HttpGet("{tourId}")]
        [RequestHeaderMatchesMediaType("Accept",new[]
        { "application/vnd.marvin.tourwithshows+json" })]
        public async Task<IActionResult> GetTourWithShows(Guid tourId)
        {
            return await GetSpecificTour<TourWithShows>(tourId, true);
        }

        [HttpGet("{tourId}")]
        [RequestHeaderMatchesMediaType("Accept", new[]
        { "application/vnd.marvin.tourwithestimatedprofitsandshows+json"})]
        public async Task<IActionResult> GetTourWithEstimatedProfitsAndShows(Guid tourId)
        {
            return await GetSpecificTour<TourWithEstimatedProfitsAndShows>(tourId, true);
        }

        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] { "application/vnd.marvin.tourwithshowsforcreation+json"})]
        public async Task<IActionResult> AddTourWithShows(
            [FromBody] TourWithShowsForCreation tour)
        {
            if(tour == null)
            {
                return BadRequest();
            }
            return await AddSpecificTour(tour);
        }


        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
           new[] { "application/vnd.marvin.tourwithmanagerandshowsforcreation+json" })]
        public async Task<IActionResult> AddTourWithManagerAndShows(
           [FromBody] TourWithManagerAndShowsForCreation tour)
        {
            if (tour == null)
            {
                return BadRequest();
            }
            return await AddSpecificTour(tour);
        }




        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type", 
            new[] {"application/json",
                "application/vnd.marvin.tourforcreation+json" })]
        public async Task<IActionResult> AddTour([FromBody]TourForCreation tour)
        {
            if(tour == null)
            {
                return BadRequest();
            }

            return await AddSpecificTour(tour);
        }

        [HttpPost]
        [RequestHeaderMatchesMediaType("Content-Type",
            new[] { "application/vnd.marvin.tourwithmanagerforcreation+json" })]
        public async Task<IActionResult> AddTourWithManager(
            [FromBody]TourWithManagerForCreation tour)
        {
            if (tour == null)
            {
                return BadRequest();
            }

            return await AddSpecificTour(tour);
        }

        [HttpPatch("{tourId}")]
        public async Task<IActionResult> PartiallyUpdateTour(Guid tourId,
            [FromBody] JsonPatchDocument<TourForUpdate> jsonPatchDocument)
        {
            if(jsonPatchDocument == null)
            {
                return BadRequest();
            }

            var tourFromRepo = await _tourManagementRepository.GetTour(tourId);

            if(tourFromRepo == null)
            {
                return BadRequest();
            }

            var tourToPatch = Mapper.Map<TourForUpdate>(tourFromRepo);

            jsonPatchDocument.ApplyTo(tourToPatch);

            Mapper.Map(tourToPatch, tourFromRepo);

            await _tourManagementRepository.UpdateTour(tourFromRepo);

            if(!await _tourManagementRepository.SaveAsync())
            {
                throw new Exception("Updating a tour failed on save.");
            }

            return NoContent();

        }

        public async Task<IActionResult> AddSpecificTour<T>(T tour) where T:class
        {
            var tourEntity = Mapper.Map<Entities.Tour>(tour);

            if(tourEntity.ManagerId == Guid.Empty)//a8dd37c5-1c37-45d7-830b-59a1be2a3c52
            {
                tourEntity.ManagerId = new Guid("fec0a4d6-5830-4eb8-8024-272bd5d6d2bb");
            }

            await _tourManagementRepository.AddTour(tourEntity);

            if(!await _tourManagementRepository.SaveAsync())
            {
                throw new Exception("Adding a tour failed on save.");
            }

            var tourToReturn = Mapper.Map<Tour>(tourEntity);

            return CreatedAtRoute("GetTour", new { tourId = tourToReturn.TourId }, tourToReturn);

        }



        public async Task<IActionResult> GetSpecificTour<T>(Guid tourId,
            bool includeShows=false) where T:class
        {
            var tourFromRepo = await _tourManagementRepository.GetTour(tourId,includeShows);

            if(tourFromRepo == null)
            {
                return BadRequest();
            }

            return Ok(Mapper.Map<T>(tourFromRepo));
        }



    }
}
