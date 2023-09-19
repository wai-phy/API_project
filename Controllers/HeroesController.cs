using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoApi.Models;
using TodoApi.Repositories;

namespace TodoApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HeroesController : ControllerBase
    {
        private readonly IRepositoryWrapper _repositoryWrapper;

        public HeroesController(IRepositoryWrapper RW)
        {
            _repositoryWrapper = RW;
        }

        // GET: api/Heroes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Hero>>> GetHeroes()
        {
            var heroItems =  await _repositoryWrapper.Hero.FindAllAsync();
            return Ok(heroItems);
        }

        // GET: api/Heroes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Hero>> GetHero(long id)
        {
            var hero = await _repositoryWrapper.Hero.FindByIDAsync(id);
            if (hero == null)
            {
                return NotFound();
            }
            return hero;
        }

        // PUT: api/Heroes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutHero(long id, Hero hero)
        {
            if (id != hero.HeroId)
            {
                return BadRequest();
            }

            Hero? objHero;
            try
            {
                objHero = await _repositoryWrapper.Hero.FindByIDAsync(id);
                if (objHero == null) 
                    throw new Exception("Invalid Hero ID");
                
                objHero.HeroName = hero.HeroName;
                
                await _repositoryWrapper.Hero.UpdateAsync(objHero);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!HeroExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/Heroes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Hero>> PostHero(Hero hero)
        {
            await _repositoryWrapper.Hero.CreateAsync(hero, true);
            return CreatedAtAction(nameof(GetHero), new { id = hero.HeroId }, hero);
        }

        // DELETE: api/Heroes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteHero(long id)
        {
            var hero = await _repositoryWrapper.Hero.FindByIDAsync(id);
            if (hero == null)
            {
                return NotFound();
            }
            await _repositoryWrapper.Hero.DeleteAsync(hero, true);
            
            return NoContent();
        }

        
        [HttpPost("search/{term}")]
        public async Task<ActionResult<IEnumerable<Hero>>>  SearchHero(string term)
        {
            var empList = await _repositoryWrapper.Hero.SearchHero(term);
            return Ok(empList);           
        }

        [HttpPost("searchhero")]
        public async Task<ActionResult<IEnumerable<Hero>>>  SearchHeroMultiple(HeroSearchPayload SearchObj)
        {
            var empList = await _repositoryWrapper.Hero.SearchHeroMultiple(SearchObj);
            return Ok(empList);           
        }

        private bool HeroExists(long id)
        {
            return _repositoryWrapper.Hero.IsExists(id);
        }
    }
}
