namespace Wasalnyy.PL.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Driver")]
    public class DriverController : ControllerBase
    {
        private readonly IDriverService _driverService;
        private readonly ITripService _tripService;

        public DriverController(IDriverService driverService, ITripService tripService)
        {
            _driverService = driverService;
            _tripService = tripService;
        }


        [ServiceFilter(typeof(WasalnyyOnlineActionFilter))]
        [HttpPost("SetAsAvailable")]
        public async Task<IActionResult> SetAsAvailableAsync([FromBody] Coordinates coordinate)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _driverService.SetDriverAvailableAsync(driverId, coordinate);
            return Ok();
        }

        [ServiceFilter(typeof(WasalnyyOnlineActionFilter))]
        [HttpPost("SetAsUnAvailable")]
        public async Task<IActionResult> SetAsUnAvailableAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (driverId == null)
                return Unauthorized();

            await _driverService.SetDriverUnAvailableAsync(driverId);
            return Ok();

        }

        [ServiceFilter(typeof(WasalnyyOnlineActionFilter))]
        [HttpPost("UpdateLocation")]
        public async Task<IActionResult> UpdateLocationAsync([FromBody] Coordinates coordinate)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            await _driverService.UpdateLocationAsync(driverId, coordinate);
            return Ok();
        }

        [HttpGet("TripsHistory")]
        public async Task<IActionResult> GetTripsHistoryAsync(
            [FromQuery] string orderBy = "RequestedDate",
            [FromQuery] bool descending = false,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            Expression<Func<Trip, object>> orderByExpression = orderByExpression => orderByExpression.RequestedDate;
            orderBy = orderBy.ToLower();

            if (string.Equals(orderBy, "StartDate".ToLower()))
                orderByExpression = orderByExpression => orderByExpression.StartDate!;

            if (string.Equals(orderBy, "ArrivalDate".ToLower()))
                orderByExpression = orderByExpression => orderByExpression.ArrivalDate!;

            if (string.Equals(orderBy, "Price".ToLower()))
                orderByExpression = orderByExpression => orderByExpression.Price;

            return Ok(await _tripService.GetAllDriverTripsPaginatedAsync(driverId, orderByExpression, descending, pageNumber, pageSize));
        }

        [HttpPut("UpdateInfo")]
        public async Task<IActionResult> UpdateDriverAsync([FromBody] DriverUpdateDto driverUpdate)
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = await _driverService.UpdateDriverInfo(driverId, driverUpdate);
            if (!result)
                return NotFound(new { Message = "Driver not found." });

            return Ok(new { Message = "Driver information updated successfully." });
        }

        [HttpGet("Vehicle")]
        public async Task<IActionResult> GetVehicleInfoAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var vehicle = await _driverService.GetDriverVehicleInfoAsync(driverId);
            if (vehicle == null)
                return NotFound(new { Message = "Vehicle info not found." });

            return Ok(vehicle);
        }

        [HttpGet("Status")]
        public async Task<IActionResult> GetStatusAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var status = await _driverService.GetDriverStatusAsync(driverId);
            return Ok(status);
        }

        [HttpGet("ProfileImage")]
        public async Task<IActionResult> GetProfileImageAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var image = await _driverService.DriverProfileImageAsync(driverId);
            return Ok(image);
        }

        [HttpGet("Name")]
        public async Task<IActionResult> GetDriverNameAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var name = await _driverService.DriverNameAsync(driverId);
            return Ok(name);
        }

        [HttpGet("CompletedTripsCount")]
        public async Task<IActionResult> GetTotalCompletedTripsAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null) return Unauthorized();

            var tripsCount = await _driverService.GetTotalCompletedTripsAsync(driverId);
            return Ok(tripsCount);
        }

        [HttpGet("Profile")]
        public async Task<IActionResult> GetProfileAsync()
        {
            var driverId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (driverId == null)
                return Unauthorized();

            var driver = await _driverService.GetByIdAsync(driverId);
            return Ok(driver);
        }
    }
}
