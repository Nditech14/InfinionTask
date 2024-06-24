using Application.Contract;
using Application.Dto;
using AutoMapper;
using Domain.Entity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace InfinionTask.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {

        private readonly IProductRepository productRepository;
        private readonly IMapper _mapper;
        public ProductController(IProductRepository productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            _mapper = mapper;
        }
       
        [HttpGet("GetAll")]
        [Authorize]
        public async Task<IActionResult> GetAllProduct()
        {
            try
            {
                var existingProduct = await productRepository.GetAllProduct();
                if (existingProduct == null)
                {
                    return NotFound();
                }

                var productdto = _mapper.Map<List<ProductDto>>(existingProduct);
                return Ok(productdto);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error{ex.Message}");
            }


        }
        [HttpGet("GetById/{id}")]
        [Authorize]
        public async Task<IActionResult> GetById([FromRoute] int id)
        {
            try
            {
                var existingProduct = await productRepository.GetProductById(id);
                if (existingProduct == null)
                {
                    return NotFound($"Proudct with {id} not found");
                }

                var productdto = _mapper.Map<ProductDto>(existingProduct);
                return Ok(productdto);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error{ex.Message}");
            }


        }
        [HttpGet("GetByName/{name}")]
        [Authorize]
        public async Task<IActionResult> GetByName([FromRoute] string name)
        {
            try
            {
                var existingProduct = await productRepository.GetProductByName(name);
                if (existingProduct == null)
                {
                    return NotFound($"Proudct with {name} not found");
                }

                var productdto = _mapper.Map<ProductDto>(existingProduct);
                return Ok(productdto);


            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error{ex.Message}");
            }


        }


        [HttpPost("Create")]
        [Authorize]
        public async Task<IActionResult> Create([FromBody] AddProductDto addProductDto)
        {
            try
            {
                var mappedentity = _mapper.Map<Product>(addProductDto);
                var existingProduct = await productRepository.GetProductByName(mappedentity.Name);

                if (existingProduct != null)
                {
                    return Conflict($"Prouct {mappedentity.Name} already exist");
                }

                await productRepository.createProduct(mappedentity);
                var productdto = _mapper.Map<ProductDto>(mappedentity);
                return CreatedAtAction(nameof(GetByName), new { name = productdto.Name }, productdto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error:{ex.Message}");
            }
        }

        [HttpPut("Update/{id}")]
        [Authorize]
        public async Task<IActionResult> Update([FromRoute] int id, [FromBody] ProductUpdateDto updateProductDto)
        {
            try
            {

                var existingProduct = await productRepository.updateProduct(id, _mapper.Map<Product>(updateProductDto));

                if (existingProduct == null)
                {
                    return NotFound($"Product with {id} is not found");
                }


                var productdto = _mapper.Map<ProductDto>(existingProduct);
                return CreatedAtAction(nameof(GetByName), new { name = productdto.Name }, productdto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error:{ex.Message}");
            }

        }
        [HttpDelete("Delete/{id}")]
        [Authorize]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var existingProduct = await productRepository.GetProductById(id);
            if (existingProduct == null)
            {
                return NotFound($"Product  with {id} not found");
            }

            await productRepository.DeleteProduct(id);
            var productdto = _mapper.Map<ProductDto>(existingProduct);
            return Ok($"{productdto.Name} Deleted sucessfully");
        }


        [HttpGet("GetByPageIndex")]
        [Authorize]

        public async Task<IActionResult> GetProducts(int pageIndex = 1, int pageSize = 10, string category = null)
        {
            try
            {
                var products = await productRepository.GetProducts(pageIndex, pageSize, category);
                return Ok(products);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }


    }
}
