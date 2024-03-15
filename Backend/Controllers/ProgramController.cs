using MSSPAPI.Helpers;
using MSSPAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace MSSPAPI.Controllers
{
    public class ProgramController : ApiController
    {

        [HttpPost]
        [ResponseType(typeof(Program))]
        public IHttpActionResult PostProgram(Program program)
        {
            try
            {
                bool isSuccess = DBOperations.InsertUpdateProgram(ref program, "C");

                if (isSuccess)
                {
                    foreach (Product product in program.Products)
                    {
                        int IdProduct = 0;
                        product.IdProgram = program.Id;

                        bool isSavedProduct = DBOperations.InsertUpdateDeleteProduct(product, "C", out IdProduct);

                        if (isSavedProduct)
                        {
                            product.Id = IdProduct;

                            foreach (Rate rate in product.Rates)
                            {
                                int IdRate = 0;
                                rate.IdProduct = product.Id;
                                rate.IdProgram = program.Id;

                                bool isSavedRate = DBOperations.InsertUpdateRate(rate, "C", out IdRate);

                                if (isSavedRate)
                                {
                                    rate.Id = IdRate;
                                }
                            }

                        }
                    }
                }

                return Ok(program);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
