using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ECommerceLiteEntity.Models;
using ECommerceLiteBLL.Repository;
using ECommerceLiteUI.Models;
using Mapster;
using ECommerceLiteBLL.Settings;
using System.IO;
using PagedList;
using ECommerceLiteBLL.Account;

namespace ECommerceLiteUI.Controllers
{
    public class ProductController : Controller
    {
        //GLOBAL ZONE
        ProductRepo myProductRepo = new ProductRepo();
        CategoryRepo myCategoryRepo = new CategoryRepo();
        ProductPictureRepo myProductPictureRepo = new ProductPictureRepo();

        public ActionResult ProductList(int page = 1, string search = "", bool isNew = false)
        {
            List<SelectListItem> subCategories = new List<SelectListItem>();
            myCategoryRepo.Queryable()
                .Where(x => x.BaseCategoryId != null)
                .ToList().ForEach(x => subCategories.Add(new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }));
            ViewBag.CategoryList = subCategories;

            List<Product> allProductList = new List<Product>();

            if (string.IsNullOrEmpty(search))
            {
                allProductList = myProductRepo.GetAll();
            }

            else
            {
                allProductList =
                    myProductRepo.Queryable()
                    .Where(x => x.ProductName.Contains(search)).ToList();
            }

            if (isNew)
            {
                allProductList = myProductRepo.GetAll();
                allProductList = allProductList.Where(x =>
                x.RegisterDate >= DateTime.Now.AddDays(-1)).ToList();

            }

            var user = MembershipTools.GetNameSurname();
            LogManager.LogMessage("geldi", userInfo: user, pageInfo: "Product/ProductList");
            return View(allProductList.ToPagedList(page, 3));
        }

        [HttpGet]
        public ActionResult Create()
        {
            List<SelectListItem> subCategories = new List<SelectListItem>();
            myCategoryRepo.Queryable()
                .Where(x => x.BaseCategoryId != null).ToList()
                .ForEach(x => subCategories.Add(new SelectListItem()
                {
                    Text = x.CategoryName,
                    Value = x.Id.ToString()
                }));
            ViewBag.CategoryList = subCategories;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductViewModel model)
        {
            try
            {
                List<SelectListItem> subCategories = new List<SelectListItem>();
                myCategoryRepo.Queryable()
                    .Where(x => x.BaseCategoryId != null).ToList()
                    .ForEach(x => subCategories.Add(new SelectListItem()
                    {
                        Text = x.CategoryName,
                        Value = x.Id.ToString()
                    }));
                ViewBag.CategoryList = subCategories;

                if (!ModelState.IsValid)
                {
                    ModelState.AddModelError("", "Data entries must be correct!");
                    return View(model);
                }

                //Mapleme yapıldı.
                Product newProduct = model.Adapt<Product>();
                int insertResult = myProductRepo.Insert(newProduct);
                if (insertResult > 0)
                {
                    //Ürünün fotoğrafları eklensin
                    if (model.Files.Any())
                    {
                        ProductPicture productPicture = new ProductPicture();
                        productPicture.ProductId = newProduct.Id;
                        productPicture.RegisterDate = DateTime.Now;
                        int counter = 1;
                        foreach (var item in model.Files)
                        {
                            if (item != null && item.ContentType.Contains("image") && item.ContentLength > 0)
                            {

                                string filename =
                                    SiteSettings.UrlFormatConverter(model.ProductName).ToLower().Replace("-", "");
                                string extName = Path
                                    .GetExtension(item.FileName);

                                string guid = Guid.NewGuid()
                                    .ToString().Replace("-", "");
                                var directoryPath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}");
                                var filePath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}/") + filename + counter + "-" + guid + extName;
                                if (!Directory.Exists(directoryPath))
                                {
                                    Directory.CreateDirectory(directoryPath);
                                }
                                item.SaveAs(filePath);
                                if (counter == 1)
                                {
                                    productPicture.ProductPicture1
                                        = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;

                                }
                                if (counter == 2)
                                {
                                    productPicture.ProductPicture2
                                        = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;

                                }
                                if (counter == 3)
                                {
                                    productPicture.ProductPicture3
                                        = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;

                                }
                                if (counter == 4)
                                {
                                    productPicture.ProductPicture4
                                        = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;

                                }
                                if (counter == 5)
                                {
                                    productPicture.ProductPicture5
                                        = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;

                                }
                            }
                            counter++;
                        }

                        int pictureInsertResult = myProductPictureRepo.Insert(productPicture);
                        if (pictureInsertResult > 0)
                        {
                            return RedirectToAction("ProductList", "Product");
                        }
                        else
                        {
                            ModelState.AddModelError("", "The product has been added, but an error occurred while adding the photos of the product! Try again to add a photo!");
                            return View(model);
                        }
                    }
                    else
                    {
                        return RedirectToAction("ProductList", "Product");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Error occurred while adding a product!Try Again!");
                    //ex loglanacak
                    return View(model);
                }
            }
            catch (Exception ex)
            {

                ModelState.AddModelError("", "Unexpected error occurred!");
                //ex loglanacak++++
                var user = MembershipTools.GetNameSurname();
                LogManager.LogMessage(ex.ToString(),
                    userInfo: user, pageInfo: "Product/Create");
                return View(model);

            }
        }

        public ActionResult CategoryProducts()
        {
            try
            {
                var list = myCategoryRepo.GetBaseCategoriesProductCount();
                return View(list);
            }
            catch (Exception ex)
            {
                var user = MembershipTools.GetNameSurname();
                LogManager.LogMessage(ex.ToString(),
                    userInfo: user, pageInfo: "Product/CategoryProducts");

                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductViewModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["ProductModalError"] = "Data entries must be correct!";
                    return RedirectToAction("ProductList", "Product");
                }
                var product = myProductRepo.GetById(model.Id);
                product.ProductName = model.ProductName;
                product.Description = model.Description;
                product.Quantity = model.Quantity;
                product.Price = model.Price;
                product.CategoryId = model.CategoryId;
                //if (model.Files.Any())
                //{
                //    //Önce sildik
                //    var pictureList =
                //          myProductPictureRepo.Queryable()
                //          .Where(x => x.ProductId == model.Id).ToList();
                //    foreach (var item in pictureList)
                //    {
                //        myProductPictureRepo.Delete(item);
                //    }
                //    //sonra yeni eklediklerini oluşturacağız.
                //    ProductPicture productPicture = new ProductPicture();
                //    productPicture.ProductId = model.Id;
                //    productPicture.RegisterDate = DateTime.Now;
                //    int counter = 1;
                //    foreach (var item in model.Files)
                //    {

                //        if (item != null && item.ContentType.Contains("image") && item.ContentLength > 0)
                //        {

                //            string filename = SiteSettings.UrlFormatConverter(model.ProductName).ToLower().Replace("-", "");
                //            string extName = Path
                //                .GetExtension(item.FileName);

                //            string guid = Guid.NewGuid()
                //                .ToString().Replace("-", "");
                //            var directoryPath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}");
                //            var filePath = Server.MapPath($"~/ProductPictures/{filename}/{model.ProductCode}/") + filename + counter + "-" + guid + extName;
                //            if (!Directory.Exists(directoryPath))
                //            {
                //                Directory.CreateDirectory(directoryPath);
                //            }
                //            item.SaveAs(filePath);
                //            if (counter == 1)
                //            {
                //                productPicture.ProductPicture1 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 2)
                //            {
                //                productPicture.ProductPicture2 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 3)
                //            {
                //                productPicture.ProductPicture3 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 4)
                //            {
                //                productPicture.ProductPicture4 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }
                //            if (counter == 5)
                //            {
                //                productPicture.ProductPicture5 = $"/ProductPictures/{filename}/{model.ProductCode}/" + filename + counter + "-" + guid + extName;
                //            }


                //        }
                //        counter++;
                //    }

                //    int pictureInsertResult =
                //        myProductPictureRepo.Insert(productPicture);
                //    if (pictureInsertResult == 0)
                //    {
                //        TempData["ProductModalError"] = "The product has been added, but an error occurred while adding the photos of the product.Try again to add a photo!";
                //    }
                //    else
                //    {
                //        TempData["ProductModalError"] = string.Empty;
                //    }
                //}

                int updateResult = myProductRepo.Update();
                if (updateResult > 0)
                {
                    return RedirectToAction("ProductList", "Product");
                }
                else
                {
                    //geçici return
                    return RedirectToAction("ProductList", "Product");

                }

            }
            catch (Exception ex)
            {
                var user = MembershipTools.GetNameSurname();
                LogManager.LogMessage(ex.ToString(),
                    userInfo: user, pageInfo: "Product/Edit");
                //geçici
                return RedirectToAction("ProductList", "Product");
            }
        }
        public JsonResult GetProductDetails(int id)
        {
            var product = myProductRepo.GetById(id);
            if (product != null)
            {
                var data = product.Adapt<ProductViewModel>();
                return Json(new { isSuccess = true, data },
                    JsonRequestBehavior.AllowGet);
            }
            return Json(new { isSuccess = false },
                JsonRequestBehavior.AllowGet);
        }
    }
}