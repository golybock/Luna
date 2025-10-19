    #!/bin/bash
    echo "Waiting for Elasticsearch to be ready..."
    sleep 10

    echo "Creating 'pages' index..."
    curl -X PUT "http://elasticsearch:9200/pages" -H 'Content-Type: application/json' -d'
    {
      "settings": {
        "number_of_shards": 1,
        "number_of_replicas": 0
      },
      {
        "mappings": {
          "properties": {
            "blocks": {
              "type": "nested",
              "properties": {
                "blockId": { "type": "keyword" },
                "pageId": { "type": "keyword" },
                "type": { "type": "keyword" },
                "content": { "type": "text" }
              }
            },
            "pageId": { "type": "keyword" },
            "title": { "type": "text" },
            "description": { "type": "text" },
            "workspaceId": { "type": "keyword" },
            "updatedAt": { "type": "date" }
          }
        }
      }
    }'

    echo "Indexes initialized."