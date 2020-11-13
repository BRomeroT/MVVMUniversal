using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebAPI.BL;

namespace WebAPI.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class CrudController : ControllerBase
    {
        readonly CrudBL crudBL;
        public CrudController(CrudBL crudBL) => this.crudBL = crudBL;

        [HttpGet("GetSections")]
        public IActionResult GetSections()
        {
            return Ok((from s in crudBL.GetSections()
                      select (SharedAPIModel.Section)s).GetEnumerator());
        }

        [HttpPut("AddSection")]
        public IActionResult AddSection(SharedAPIModel.Section section)
        {
            var res = crudBL.AddSection(section);
            if (res) return StatusCode(StatusCodes.Status201Created);
            return Conflict();
        }

        [HttpDelete("DeleteSection")]
        public IActionResult DeleteSection(int id)
        {
            var res = crudBL.DeleteSection(id);
            if (res) return Ok();
            return NotFound();
        }

        [HttpPost("UpdateSection")]
        public IActionResult UpdateSection(SharedAPIModel.Section section)
        {
            var res = crudBL.UpdateSection(section);
            if (res) return Ok(section.Id);
            return Conflict();
        }

        [HttpGet("GetItems")]
        public IActionResult GetItems(int idSection)
        {
            var res = crudBL.GetItems(idSection).Select(i => (SharedAPIModel.Item)i).ToList();
            if (res.Count == 0) return NotFound(res);
            return Ok(res);
        }

        [HttpPut("AddItem")]
        public IActionResult AddItem(SharedAPIModel.Item item)
        {
            var res = crudBL.AddItem(item);
            if (res) return StatusCode(StatusCodes.Status201Created);
            return Conflict();
        }

        [HttpDelete("DeleteItem")]
        public IActionResult DeleteItem(int id)
        {
            var res = crudBL.DeleteItem(id);
            if (res) return Ok();
            return NotFound();
        }

    }
}
