using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace MSSPAPI.Controllers
{
    /// <summary>
    /// API CRUD para la marca o branch used for enrollment 
    /// </summary>
    public class BranchesController : ApiController
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// Obtiene el todas las marcas
        /// </summary>
        /// <param></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetBranches")]
        public IHttpActionResult GetBranches()
        {
            try
            {
                List<Marcas> dc = DBOperations.GetAllMarcas();
                return Ok(dc);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Obtiene una sola marca mediante su id
        /// </summary>
        /// <param name="marca"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("api/GetBranches/{marca}")]
        public IHttpActionResult GetBranches(string marca)
        {
            try
            {
                Marcas dc = DBOperations.GetOneBranch(marca);
                return Ok(dc);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Insertar una nueva marca
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/InsertBranch")]
        public IHttpActionResult InsertBranch(Marcas ms)
        {
            try
            {
                DBOperations.InsertBranches(ms);
                return Ok(ms);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Actualizar una marca
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("api/UpdateBranches")]
        public IHttpActionResult UpdateBranches(Marcas ms)
        {
            try
            {
                DBOperations.UpdateBranches(ms);
                return Ok(ms);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        /// <summary>
        /// Eliminar un branch
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/DeleteBranch")]
        public IHttpActionResult DeleteBranch(int id)
        {
            try
            {
                DBOperations.DeleteBranches(id);
                return Ok();
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }
    }
}