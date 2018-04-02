using System;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;

namespace E_CommerceApp
{
    /// <summary>
    /// Contains methods for obtaining data from the database
    /// </summary>
    public class DBOps
    {
        #region Connection Strings
        static string cartConn = ConfigurationManager.ConnectionStrings["CartConnectionString"].ToString();
        static string userConn = ConfigurationManager.ConnectionStrings["UserConnectionString"].ToString();
        static string productConn = ConfigurationManager.ConnectionStrings["ProductsConnectionString"].ToString();
        #endregion

        /// <summary>
        /// Obtain the ID associated with the latest row in the "purchases" table
        /// </summary>
        /// <returns>The ID of the last inserted row in the "purchases" table</returns>
        public static int GetLatestEntry()
        {
            int id = -1;
            string queryString = @"SELECT TOP 1 Id FROM purchases ORDER BY Id DESC";
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        id = Convert.ToInt32(reader["Id"].ToString());
                    }

                }
            }

            return id;
        }

        /// <summary>
        /// Assigns a new cart ID to the specified user
        /// </summary>
        /// <param name="userEmail"></param>
        public static void ReassignUserCart(string userEmail)
        {
            string queryString = @"UPDATE userInfo SET latest_cart_id=@cartID WHERE Id=@userID";
            int userID = GetUserID(userEmail);
            using (SqlConnection conn = new SqlConnection(userConn))
            {
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@cartID", SqlDbType.Int).Value = GetLatestEntry() + 1;
                    comm.Parameters.AddWithValue("@userID", SqlDbType.Int).Value = userID;
                    conn.Open();
                    comm.ExecuteNonQuery();
                }

            }

        }

        /// <summary>
        /// Associates the specified cart to the given user
        /// </summary>
        /// <param name="userEmail">The e-mail address of the user to take ownership of the cart</param>
        /// <param name="cartID">The cart ID of the cart to be assigned to the user</param>
        public static void ReassignUserCart(string userEmail, int cartID)
        {
            string queryString = @"UPDATE userInfo SET latest_cart_id=@cartID WHERE Id=@userID";
            int userID = GetUserID(userEmail);

            using (SqlConnection conn = new SqlConnection(userConn))
            {
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@cartID", SqlDbType.Int).Value = cartID;
                    comm.Parameters.AddWithValue("@userID", SqlDbType.Int).Value = userID;
                    conn.Open();
                    comm.ExecuteNonQuery();
                }

            }
        }

        /// <summary>
        /// Assigns the cart in the products database to the specified user.
        /// </summary>
        /// <param name="userEmail">The e-mail address of the user to take ownership of the cart</param>
        public static void RegisterCart(string userEmail)
        {
            string queryString = @"UPDATE purchases SET customer=@userID WHERE Id=@cartID";
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@cartID", SqlDbType.Int).Value = GetLatestEntry(GetUserID(userEmail));
                    comm.Parameters.AddWithValue("@userID", SqlDbType.Int).Value = userEmail;
                    conn.Open();
                    comm.ExecuteNonQuery();
                }

            }
        }

        /// <summary>
        /// Obtains the user's login credentials from the database.
        /// </summary>
        /// <param name="userEmail">The specified e-mail address by the user.</param>
        /// <param name="Username">The variable where the obtained username will be stored.</param>
        /// <param name="Password">The variable where the obtained password will be stored.</param>
        public static void GetLoginDetails(string userEmail, out string Username, out string Password)
        {
            Username = string.Empty;
            Password = string.Empty;
            using (SqlConnection conn = new SqlConnection(userConn))
            {
                string command = @"SELECT email,password FROM userInfo WHERE email=@id";
                using (SqlCommand comm = new SqlCommand(command, conn))
                {
                    conn.Open();
                    comm.Parameters.AddWithValue("@id", SqlDbType.NVarChar).Value = userEmail;
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Username = reader["email"].ToString();
                            Password = reader["password"].ToString();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the cart associated with the specified ID.
        /// </summary>
        /// <param name="cartID">The cart's cart id.</param>
        /// <returns>The items in the user's cart</returns>
        public static string[] GetCartItems(int cartID)
        {
            string[] items = new string[5];

            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                string command = @"SELECT items,prices,quants,totalCount,totalPrice " +
                    "FROM purchases WHERE Id=@id";
                using (SqlCommand comm = new SqlCommand(command, conn))
                {
                    conn.Open();
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = cartID;
                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            items[0] = reader["items"].ToString();
                            items[1] = reader["prices"].ToString();
                            items[2] = reader["quants"].ToString();
                            items[3] = reader["totalCount"].ToString();
                            items[4] = reader["totalPrice"].ToString();
                        }
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Gets the ID associated with specified user's cart from the database
        /// </summary>
        /// <param name="userID">The ID of the user who's latest entry is to be retrived from the database</param>
        /// <returns>The value of the "recently assigned cart" field associated with the given user ID</returns>
        public static int GetLatestEntry(int userID)
        {
            int id = -1;
            string queryString = @"SELECT latest_cart_id FROM userInfo WHERE Id=@id";
            using (SqlConnection conn = new SqlConnection(userConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = userID;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        id = Convert.ToInt32(reader["latest_cart_id"].ToString());
                    }

                }
            }

            return id;
        }

        /// <summary>
        /// Get the primary key associated with the user's details in the database table.
        /// </summary>
        /// <param name="userMail">The user's e-mail</param>
        /// <returns>The user's ID in the database table</returns>
        public static int GetUserID(string userMail)
        {
            int id = -1;
            string queryString = @"SELECT Id FROM userInfo WHERE email=@mail";
            using (SqlConnection conn = new SqlConnection(userConn))
            {
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    conn.Open();
                    comm.Parameters.AddWithValue("@mail", SqlDbType.NVarChar).Value = userMail;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        id = Convert.ToInt32(reader["Id"].ToString());
                    }

                }
            }

            return id;
        }

        /// <summary>
        /// Get the items in the items in the user's current cart.
        /// </summary>
        /// <param name="cartID">The ID of the cart</param>
        /// <returns>An array containing the details of the user's cart</returns>
        public static string[] GetUserCart(int cartID)
        {
            string queryString = @"SELECT items,prices,quants,totalCount,totalPrice FROM purchases WHERE Id=@id";
            string[] data = new string[5];
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = cartID;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Get the items in the items in the user's current cart.
        /// </summary>
        /// <param name="refKey">The reference key associated with the cart</param>
        /// <returns>An array containing the details of the user's cart</returns>
        public static string[] GetUserCart(string refKey)
        {
            string queryString = @"SELECT items,prices,quants,totalCount,totalPrice FROM purchases WHERE reference_key=@refKey";
            string[] data = new string[5];
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@refKey", SqlDbType.NVarChar).Value = refKey;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Get the items in the items in the user's current cart.
        /// </summary>
        /// <param name="cartID">The ID of the cart</param>
        /// <param name="userEmail">The e-mail associated with the cart</param>
        /// <returns>An array containing the details of the user's cart</returns>
        public static string[] GetUserCart(int cartID, string userEmail)
        {
            string queryString = @"SELECT items,prices,quants,totalCount,totalPrice FROM purchases WHERE Id=@id and customer=@customer";
            string[] data = new string[5];
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = cartID;
                    comm.Parameters.AddWithValue("@customer", SqlDbType.NVarChar).Value = userEmail;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the totals from the user's cart details
        /// </summary>
        /// <param name="cartID">The ID associated with the cart</param>
        /// <param name="userEmail">The E-mail associated with the cart</param>
        /// <returns></returns>
        public static string[] GetUserCartTotals(int cartID, string userEmail)
        {
            string queryString = @"SELECT totalCount,totalPrice FROM purchases WHERE Id=@id and customer=@customer";
            string[] data = new string[2];
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = cartID;
                    comm.Parameters.AddWithValue("@customer", SqlDbType.NVarChar).Value = userEmail;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the totals from the user's cart details
        /// </summary>
        /// <param name="cartID">The ID associated with the cart</param>
        /// <returns></returns>
        public static string[] GetUserCartTotals(int cartID)
        {
            string queryString = @"SELECT totalCount,totalPrice FROM purchases WHERE Id=@id";
            string[] data = new string[2];
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = cartID;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the totals from the user's cart details
        /// </summary>
        /// <param name="refKey">The reference key associated with the cart</param>
        /// <returns></returns>
        public static string[] GetUserCartTotals(string refKey)
        {
            string queryString = @"SELECT totalCount,totalPrice FROM purchases WHERE reference_key=@refKey";
            string[] data = new string[2];
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@refKey", SqlDbType.NVarChar).Value = refKey;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the items in the specified user's cart
        /// </summary>
        /// <param name="userEmail">The e-mail address that the user uses to login</param>
        /// <returns>An array containing all the details in the specified user's cart</returns>
        public static string[] GetUserCardDetails(string userEmail)
        {
            string queryString = @"SELECT card_owner,card_number,card_expiry,card_secNumber,address FROM userInfo WHERE Id=@id";
            string[] data = new string[5];
            using (SqlConnection conn = new SqlConnection(userConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = GetUserID(userEmail);
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data[0] = reader[0].ToString();
                        data[1] = reader[1].ToString();
                        data[2] = reader[2].ToString();
                        data[3] = reader[3].ToString();
                        data[4] = reader[4].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Checks for the existence of a specific cart in the purchases table
        /// </summary>
        /// <param name="cartID">The cartID associated with the cart</param>
        /// <returns>True if the cart exists</returns>
        public static bool RecordExists(int cartID)
        {
            string queryString = @"SELECT * FROM Purchases WHERE Id=@id";
            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@id", SqlDbType.Int).Value = cartID;
                    SqlDataReader reader = comm.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader.HasRows)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the total value of the cart from the database
        /// </summary>
        /// <param name="cartID">The cart ID to obtain from the requested values from</param>
        /// <returns></returns>
        public static DataTable BuildUserCartTotals(int cartID)
        {
            string[] data = GetUserCartTotals(cartID);
            int totalCount = 0;
            decimal totalPrice = 0;

            totalCount = Convert.ToInt32(data[0]);
            totalPrice = Convert.ToDecimal(data[1]);

            DataTable table = new DataTable();
            DataColumn column;

            column = new DataColumn()
            {
                ColumnName = "totalQuantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalPrice",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            table.Rows.Add(totalCount, totalPrice);


            return table;
        }

        /// <summary>
        /// Gets the total value of the cart from the database
        /// </summary>
        /// <param name="refKey">The reference key associated with the transaction to obtain the values from</param>
        /// <returns></returns>
        public static DataTable BuildUserCartTotals(string refKey)
        {
            string[] data = GetUserCartTotals(refKey);
            int totalCount = 0;
            decimal totalPrice = 0;

            totalCount = Convert.ToInt32(data[0]);
            totalPrice = Convert.ToDecimal(data[1]);

            DataTable table = new DataTable();
            DataColumn column;

            column = new DataColumn()
            {
                ColumnName = "totalQuantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalPrice",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            table.Rows.Add(totalCount, totalPrice);


            return table;
        }

        /// <summary>
        /// I don't exactly rememebr. But, I believe: Gets the total value of the specified user's cart
        /// </summary>
        /// <param name="cartID">The cart ID to obtain from the requested values from</param>
        /// <param name="userEmail">The e-mail address used by the user to login</param>
        /// <returns></returns>
        public static DataTable UserCartTotals(int cartID, string userEmail)
        {
            string[] data = GetUserCartTotals(cartID, userEmail);
            int totalCount = 0;
            decimal totalPrice = 0;

            totalCount = Convert.ToInt32(data[0]);
            totalPrice = Convert.ToDecimal(data[1]);

            DataTable table = new DataTable();
            DataColumn column;

            column = new DataColumn()
            {
                ColumnName = "totalQuantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalPrice",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            table.Rows.Add(totalCount, totalPrice);


            return table;
        }

        /// <summary>
        /// Gets the product name associated with the given SKU
        /// </summary>
        /// <param name="SKU">The SKU associated with the product</param>
        /// <returns></returns>
        public static string GetSKUEquiv(string SKU)
        {
            string queryString = @"SELECT name,sku FROM products WHERE sku=@sku";
            string data = string.Empty;
            using (SqlConnection conn = new SqlConnection(productConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@sku", SqlDbType.NVarChar).Value = SKU;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data = reader["name"].ToString();
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Builds a table containing the user's cart
        /// </summary>
        /// <param name="cartID">The ID of the user's cart.</param>
        /// <returns>A datatable representation of the user's cart</returns>
        public static DataTable BuildUserCart(int cartID)
        {
            string[] data = GetUserCart(cartID);
            int totalCount = 0;
            decimal totalPrice = 0;

            string[] items = null, prices = null, quants = null;

            if (data[0] != null)
            {
                items = data[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                prices = data[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                quants = data[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            totalCount = Convert.ToInt32(data[3]);
            totalPrice = Convert.ToDecimal(data[4]);

            DataTable table = new DataTable("userItems");
            DataColumn column;

            column = new DataColumn()
            {
                ColumnName = "item",
                AllowDBNull = false,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "sku",
                AllowDBNull = false,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "price",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "quantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalQuantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalPrice",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            Decimal originalPrice = 0;

            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    originalPrice = Convert.ToDecimal(prices[i]) / Convert.ToInt32(quants[i]);
                    table.Rows.Add(GetSKUEquiv(items[i]), items[i], originalPrice, Convert.ToInt32(quants[i]), totalCount, totalPrice);
                }
            }

            return table;
        }

        /// <summary>
        /// Builds a table containing the user's cart
        /// </summary>
        /// <param name="cartID">The ID of the user's cart.</param>
        /// <returns>A datatable representation of the user's cart</returns>
        public static DataTable BuildUserCart(string refKey)
        {
            string[] data = GetUserCart(refKey);
            int totalCount = 0;
            decimal totalPrice = 0;

            string[] items = null, prices = null, quants = null;

            if (data[0] != null)
            {
                items = data[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                prices = data[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                quants = data[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            totalCount = Convert.ToInt32(data[3]);
            totalPrice = Convert.ToDecimal(data[4]);

            DataTable table = new DataTable("userItems");
            DataColumn column;

            column = new DataColumn()
            {
                ColumnName = "item",
                AllowDBNull = false,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "sku",
                AllowDBNull = false,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "price",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "quantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalQuantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalPrice",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            Decimal originalPrice = 0;

            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    originalPrice = Convert.ToDecimal(prices[i]) / Convert.ToInt32(quants[i]);
                    table.Rows.Add(GetSKUEquiv(items[i]), items[i], originalPrice, Convert.ToInt32(quants[i]), totalCount, totalPrice);
                }
            }

            return table;
        }

        /// <summary>
        /// Builds a data table composed of details from the specified cart
        /// </summary>
        /// <param name="cartID">The ID associated with the cart</param>
        /// <param name="userEmail">The E-mail associated with the cart</param>
        /// <returns>A datatable representation of the user's cart</returns>
        public static DataTable UserCart(int cartID, string userEmail)
        {
            string[] data = GetUserCart(cartID, userEmail);
            int totalCount = 0;
            decimal totalPrice = 0;

            string[] items = null, prices = null, quants = null;

            if (data[0] != null)
            {
                items = data[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                prices = data[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                quants = data[2].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            }

            totalCount = Convert.ToInt32(data[3]);
            totalPrice = Convert.ToDecimal(data[4]);

            DataTable table = new DataTable("userItems");
            DataColumn column;

            column = new DataColumn()
            {
                ColumnName = "item",
                AllowDBNull = false,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "sku",
                AllowDBNull = false,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "price",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            column = new DataColumn()
            {
                ColumnName = "quantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalQuantity",
                AllowDBNull = false,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);
            column = new DataColumn()
            {
                ColumnName = "totalPrice",
                AllowDBNull = false,
                DataType = Type.GetType("System.Decimal")
            };
            table.Columns.Add(column);

            Decimal originalPrice = 0;

            if (items != null)
            {
                for (int i = 0; i < items.Length; i++)
                {
                    originalPrice = Convert.ToDecimal(prices[i]) / Convert.ToInt32(quants[i]);
                    table.Rows.Add(GetSKUEquiv(items[i]), items[i], originalPrice, Convert.ToInt32(quants[i]), totalCount, totalPrice);
                }
            }

            return table;
        }

        /// <summary>
        /// Obtains the specified user's credit card details
        /// </summary>
        /// <param name="userEmail">The E-mail address of the user who's card details are to be obtained</param>
        /// <returns>A data table containing the user's credit card details</returns>
        public static DataTable BuildCreditCardDetails(string userEmail)
        {
            DataTable table = new DataTable("cardDetails");
            DataColumn column;
            string[] data = GetUserCardDetails(userEmail);

            column = new DataColumn("name")
            {
                AllowDBNull = true,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            column = new DataColumn("number")
            {
                AllowDBNull = true,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);

            column = new DataColumn("exp")
            {
                AllowDBNull = true,
                DataType = Type.GetType("System.DateTime")
            };
            table.Columns.Add(column);

            column = new DataColumn("secCode")
            {
                AllowDBNull = true,
                DataType = Type.GetType("System.Int32")
            };
            table.Columns.Add(column);

            column = new DataColumn("address")
            {
                AllowDBNull = true,
                DataType = Type.GetType("System.String")
            };
            table.Columns.Add(column);

            table.Rows.Add(data[0], Convert.ToInt32(data[1]), Convert.ToDateTime(data[2]), Convert.ToInt32(data[3]), data[4]);

            return table;
        }

        /// <summary>
        /// Gets the number of the specified item present in the Products database
        /// </summary>
        /// <param name="SKU">The SKU of the product in the database</param>
        /// <returns></returns>
        public static int GetProductQuantity(string SKU)
        {
            string queryString = @"SELECT qty FROM Products WHERE sku=@sku";
            int data = 0;
            using (SqlConnection conn = new SqlConnection(productConn))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.AddWithValue("@sku", SqlDbType.NVarChar).Value = SKU;
                    SqlDataReader reader = comm.ExecuteReader();
                    if (reader.Read())
                    {
                        data = Convert.ToInt32(reader["qty"].ToString());
                    }
                }
            }

            return data;
        }

        /// <summary>
        /// Gets the quantity of the specified item in the user's cart
        /// </summary>
        /// <param name="cartID">The current cart's ID</param>
        /// <param name="itemSKU">The SKU of the item</param>
        /// <returns></returns>
        public static int GetItemQuantity(int cartID, string itemSKU)
        {
            int data = 0;

            string[] originalCartData = GetUserCart(cartID);

            string[] t_lastInsertedItem = originalCartData[0].
                Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            string[] t_lastInsertedQuant = originalCartData[2].
                Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < t_lastInsertedItem.Length; i++)
            {
                if (t_lastInsertedItem[i] == itemSKU)
                {
                    data = Convert.ToInt32(t_lastInsertedQuant[i]);
                    break;
                }
            }


            return data;
        }

        /// <summary>
        /// Obtains every transaction that is associated with the specified user
        /// </summary>
        /// <param name="UserEmail">The E-mail address of the user who's transaction histories are to be obtained</param>
        /// <returns></returns>
        private static List<string> GetTransactionHistories(string UserEmail)
        {
            List<string> Data = new List<string>();

            string queryString = @"SELECT date,reference_key,totalCount,totalPrice FROM purchases " +
                "WHERE customer=@cutsomer";
            string h_items = string.Empty;

            using (SqlConnection conn = new SqlConnection(cartConn))
            {
                using (SqlCommand comm = new SqlCommand(queryString, conn))
                {
                    comm.Parameters.Add("@cutsomer", SqlDbType.NVarChar).Value = UserEmail;
                    conn.Open();

                    using (SqlDataReader reader = comm.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            for (int i = 0; i < 4; i++)
                            {
                                h_items += reader[i] + ",";
                            }

                            Data.Add(h_items);

                            h_items = string.Empty;
                        }
                    }
                }
            }

            return Data;
        }

        /// <summary>
        /// Builds a table containing all the transactions associated with the specified user
        /// </summary>
        /// <param name="UserEmail">The E-mail address of the user who's transaction history is to be obtained</param>
        /// <returns></returns>
        public static DataTable UserTransactionsTable(string UserEmail)
        {
            List<string> DataItems = GetTransactionHistories(UserEmail);

            DataTable table = new DataTable("user");
            DataColumn column;

            int ItemCounter = 0;

            column = new DataColumn("TransactionNumber")
            {
                AllowDBNull = false,
                DataType = typeof(Int32)
            };

            table.Columns.Add(column);


            column = new DataColumn("Date")
            {
                AllowDBNull = false,
                DataType = typeof(DateTime)
            };

            table.Columns.Add(column);

            column = new DataColumn("RefNum")
            {
                AllowDBNull = false,
                DataType = typeof(String)
            };

            table.Columns.Add(column); column = new DataColumn("ItemCount")
            {
                AllowDBNull = false,
                DataType = typeof(Int32)
            };

            table.Columns.Add(column); column = new DataColumn("TotalPrice")
            {
                AllowDBNull = false,
                DataType = typeof(Decimal)
            };

            table.Columns.Add(column);


            foreach (string item in DataItems)
            {
                ItemCounter++;
                string[] ts_DataItems = DataItems.ToArray();
                ts_DataItems = item.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                table.Rows.Add(ItemCounter, Convert.ToDateTime(ts_DataItems[0]),
                    ts_DataItems[1], Convert.ToInt32(ts_DataItems[2]), Convert.ToDecimal(ts_DataItems[3]));
            }

            return table;
        }
    }
}