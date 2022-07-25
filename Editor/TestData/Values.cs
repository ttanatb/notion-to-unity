using Newtonsoft.Json.Linq;

namespace NotionToUnity.TestData
{
    public static class Values
    {
        public static JObject DbResult => JObject.Parse(@"{
  ""object"": ""list"",
  ""results"": [
    {
      ""object"": ""page"",
      ""id"": ""887617bd-51c4-4f64-8c96-fdcc47bf72b6"",
      ""created_time"": ""2022-05-15T11:36:00Z"",
      ""last_edited_time"": ""2022-05-15T11:36:00Z"",
      ""created_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""last_edited_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""cover"": null,
      ""icon"": null,
      ""parent"": {
        ""type"": ""database_id"",
        ""database_id"": ""a2d5db5c-7143-42e6-9d96-a754ea9f9194""
      },
      ""archived"": false,
      ""properties"": {
        ""Flag Toggle"": {
          ""id"": ""GHxs"",
          ""type"": ""rich_text"",
          ""rich_text"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""potato"",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""potato"",
              ""href"": null
            }
          ]
        },
        ""Description"": {
          ""id"": ""Gt%3EE"",
          ""type"": ""rich_text"",
          ""rich_text"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""Shorter description of item #2"",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""Shorter description of item #2"",
              ""href"": null
            }
          ]
        },
        ""Sprite"": {
          ""id"": ""XiSY"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Category"": {
          ""id"": ""kS%5BJ"",
          ""type"": ""select"",
          ""select"": {
            ""id"": ""dHgk"",
            ""name"": ""Fish"",
            ""color"": ""purple""
          }
        },
        ""Name"": {
          ""id"": ""title"",
          ""type"": ""title"",
          ""title"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""test_item_two"",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""test_item_two"",
              ""href"": null
            }
          ]
        }
      },
      ""url"": ""https://www.notion.so/test_item_two-887617bd51c44f648c96fdcc47bf72b6""
    },
    {
      ""object"": ""page"",
      ""id"": ""a92007a1-0548-4e02-acb1-1deaba84eacd"",
      ""created_time"": ""2022-05-14T18:45:00Z"",
      ""last_edited_time"": ""2022-05-14T18:45:00Z"",
      ""created_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""last_edited_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""cover"": null,
      ""icon"": null,
      ""parent"": {
        ""type"": ""database_id"",
        ""database_id"": ""a2d5db5c-7143-42e6-9d96-a754ea9f9194""
      },
      ""archived"": false,
      ""properties"": {
        ""Flag Toggle"": {
          ""id"": ""GHxs"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Description"": {
          ""id"": ""Gt%3EE"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Sprite"": {
          ""id"": ""XiSY"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Category"": {
          ""id"": ""kS%5BJ"",
          ""type"": ""select"",
          ""select"": null
        },
        ""Name"": {
          ""id"": ""title"",
          ""type"": ""title"",
          ""title"": []
        }
      },
      ""url"": ""https://www.notion.so/a92007a105484e02acb11deaba84eacd""
    },
    {
      ""object"": ""page"",
      ""id"": ""990edcda-6cb9-4ca4-9e97-a354f28d1bdc"",
      ""created_time"": ""2022-05-14T18:44:00Z"",
      ""last_edited_time"": ""2022-05-14T23:22:00Z"",
      ""created_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""last_edited_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""cover"": null,
      ""icon"": null,
      ""parent"": {
        ""type"": ""database_id"",
        ""database_id"": ""a2d5db5c-7143-42e6-9d96-a754ea9f9194""
      },
      ""archived"": false,
      ""properties"": {
        ""Flag Toggle"": {
          ""id"": ""GHxs"",
          ""type"": ""rich_text"",
          ""rich_text"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""test_event"",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""test_event"",
              ""href"": null
            }
          ]
        },
        ""Description"": {
          ""id"": ""Gt%3EE"",
          ""type"": ""rich_text"",
          ""rich_text"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""Very long description of an item, it’s really good I guess."",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""Very long description of an item, it’s really good I guess."",
              ""href"": null
            }
          ]
        },
        ""Sprite"": {
          ""id"": ""XiSY"",
          ""type"": ""rich_text"",
          ""rich_text"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""test_image.png"",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""test_image.png"",
              ""href"": null
            }
          ]
        },
        ""Category"": {
          ""id"": ""kS%5BJ"",
          ""type"": ""select"",
          ""select"": {
            ""id"": ""^DEk"",
            ""name"": ""Key Item"",
            ""color"": ""pink""
          }
        },
        ""Name"": {
          ""id"": ""title"",
          ""type"": ""title"",
          ""title"": [
            {
              ""type"": ""text"",
              ""text"": {
                ""content"": ""test_item_name"",
                ""link"": null
              },
              ""annotations"": {
                ""bold"": false,
                ""italic"": false,
                ""strikethrough"": false,
                ""underline"": false,
                ""code"": false,
                ""color"": ""default""
              },
              ""plain_text"": ""test_item_name"",
              ""href"": null
            }
          ]
        }
      },
      ""url"": ""https://www.notion.so/test_item_name-990edcda6cb94ca49e97a354f28d1bdc""
    },
    {
      ""object"": ""page"",
      ""id"": ""bab7c98c-0713-4afc-841f-a3c78a730705"",
      ""created_time"": ""2022-05-14T18:44:00Z"",
      ""last_edited_time"": ""2022-05-14T18:44:00Z"",
      ""created_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""last_edited_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""cover"": null,
      ""icon"": null,
      ""parent"": {
        ""type"": ""database_id"",
        ""database_id"": ""a2d5db5c-7143-42e6-9d96-a754ea9f9194""
      },
      ""archived"": false,
      ""properties"": {
        ""Flag Toggle"": {
          ""id"": ""GHxs"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Description"": {
          ""id"": ""Gt%3EE"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Sprite"": {
          ""id"": ""XiSY"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Category"": {
          ""id"": ""kS%5BJ"",
          ""type"": ""select"",
          ""select"": null
        },
        ""Name"": {
          ""id"": ""title"",
          ""type"": ""title"",
          ""title"": []
        }
      },
      ""url"": ""https://www.notion.so/bab7c98c07134afc841fa3c78a730705""
    },
    {
      ""object"": ""page"",
      ""id"": ""fba23e47-c2bc-4c9b-b1ef-9112845b5c2e"",
      ""created_time"": ""2022-05-14T18:44:00Z"",
      ""last_edited_time"": ""2022-05-14T18:44:00Z"",
      ""created_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""last_edited_by"": {
        ""object"": ""user"",
        ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
      },
      ""cover"": null,
      ""icon"": null,
      ""parent"": {
        ""type"": ""database_id"",
        ""database_id"": ""a2d5db5c-7143-42e6-9d96-a754ea9f9194""
      },
      ""archived"": false,
      ""properties"": {
        ""Flag Toggle"": {
          ""id"": ""GHxs"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Description"": {
          ""id"": ""Gt%3EE"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Sprite"": {
          ""id"": ""XiSY"",
          ""type"": ""rich_text"",
          ""rich_text"": []
        },
        ""Category"": {
          ""id"": ""kS%5BJ"",
          ""type"": ""select"",
          ""select"": null
        },
        ""Name"": {
          ""id"": ""title"",
          ""type"": ""title"",
          ""title"": []
        }
      },
      ""url"": ""https://www.notion.so/fba23e47c2bc4c9bb1ef9112845b5c2e""
    }
  ],
  ""next_cursor"": null,
  ""has_more"": false,
  ""type"": ""page"",
  ""page"": {}
}");

        public static JObject DbProperty => JObject.Parse(@"{
  ""object"": ""database"",
            ""id"": ""a2d5db5c-7143-42e6-9d96-a754ea9f9194"",
        ""cover"": null,
        ""icon"": {
            ""type"": ""emoji"",
            ""emoji"": ""💽""
        },
    ""created_time"": ""2022-05-14T18:44:00Z"",
    ""created_by"": {
    ""object"": ""user"",
    ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
},
""last_edited_by"": {
    ""object"": ""user"",
    ""id"": ""652787e7-110b-422f-af8d-600f44417ea4""
},
""last_edited_time"": ""2022-05-15T11:36:00Z"",
""title"": [
{
    ""type"": ""text"",
    ""text"": {
        ""content"": ""Item Database"",
        ""link"": null
    },
    ""annotations"": {
        ""bold"": false,
        ""italic"": false,
        ""strikethrough"": false,
        ""underline"": false,
        ""code"": false,
        ""color"": ""default""
    },
    ""plain_text"": ""Item Database"",
    ""href"": null
}
],
""properties"": {
    ""Flag Toggle"": {
        ""id"": ""GHxs"",
        ""name"": ""Flag Toggle"",
        ""type"": ""rich_text"",
        ""rich_text"": {}
    },
    ""Description"": {
        ""id"": ""Gt%3EE"",
        ""name"": ""Description"",
        ""type"": ""rich_text"",
        ""rich_text"": {}
    },
    ""Sprite"": {
        ""id"": ""XiSY"",
        ""name"": ""Sprite"",
        ""type"": ""rich_text"",
        ""rich_text"": {}
    },
    ""Category"": {
        ""id"": ""kS%5BJ"",
        ""name"": ""Category"",
        ""type"": ""select"",
        ""select"": {
            ""options"": [
            {
                ""id"": ""qyy^"",
                ""name"": ""Common"",
                ""color"": ""blue""
            },
            {
                ""id"": ""dHgk"",
                ""name"": ""Fish"",
                ""color"": ""purple""
            },
            {
                ""id"": ""^DEk"",
                ""name"": ""Key Item"",
                ""color"": ""pink""
            }
            ]
        }
    },
    ""Name"": {
        ""id"": ""title"",
        ""name"": ""Name"",
        ""type"": ""title"",
        ""title"": {}
    }
},
""parent"": {
    ""type"": ""page_id"",
    ""page_id"": ""3f3169e9-866a-4d13-ad7c-d602e7853df6""
},
""url"": ""https://www.notion.so/a2d5db5c714342e69d96a754ea9f9194"",
""archived"": false
}");

    }
}
