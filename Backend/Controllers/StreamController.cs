using Backend.DataObjects;
using Backend.Models;
using Geo.Geometries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Backend.Controllers
{
    public class StreamController : Controller
    {
        private CardDocumentDBRepository<CardModel> _cardStorage;
        private IOptions<CardConfig> _cardConfig;

        public StreamController(
            CardDocumentDBRepository<CardModel> cardStorage,
            IOptions<CardConfig> cardConfig)
        {
            _cardStorage = cardStorage;
            _cardConfig = cardConfig;
        }

        [HttpGet]
        public async Task<IActionResult> Live(Guid id)
        {
            var card = await _cardStorage.GetItemAsync(id.ToString());
            if (card == null) return NotFound();

            return Ok(card);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Add([FromBody] CardModel card)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            // Check publisher
            if (User.Identity.IsAuthenticated &&
                Guid.TryParse(User.FindFirst(ClaimTypes.NameIdentifier).Value ?? string.Empty, out Guid userId))
            {
                card.Publisher = new PublisherModel
                {
                    UserId = userId,
                    UserName = User.Identity.Name
                };
            }
            else
            {
                return BadRequest(new ErrorMessageModel("Unknown user"));
            }

            // Append date and GUID
            card.TimeStamp = DateTime.Now;
            card.Id = Guid.NewGuid();

            // Transform image or video url
            if (card.Media != null)
            {
                if (card.Media.Images != null)
                {
                    for (var i = 0; i < card.Media.Images.Length; i++)
                    {
                        card.Media.Images[i] = $"{_cardConfig.Value.BlobAddress}{card.Media.Images[i]}";
                    }
                }
            }

            await _cardStorage.CreateItemAsync(card);
            return Created($"/stream/live/{card.Id}", card);
        }

        [HttpPost]
        public async Task<IActionResult> Live([FromBody] CardQueryModel model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var geoCircle = new Circle(
                new Geo.Coordinate(model.Lat, model.Lon), 
                (model.Radius != 0) ? model.Radius : CardQueryModel.DefaultRadius
            );

            var geoEnv = geoCircle.GetBounds();
            var cardQuery = await _cardStorage.GetItemsAsync(k =>
                k.Location.Lat >= geoEnv.MinLat &&
                k.Location.Lat <= geoEnv.MaxLat &&
                k.Location.Lon >= geoEnv.MinLon &&
                k.Location.Lon <= geoEnv.MaxLon);

            return Ok(cardQuery.ToList());
        }
    }
}
