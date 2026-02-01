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
  "mappings": {
    "properties": {
      "pageId": { "type": "keyword" },
      "workspaceId": { "type": "keyword" },
      "title": { "type": "text", "analyzer": "standard" },
      "description": { "type": "text", "analyzer": "standard" },
      "content": { "type": "text", "analyzer": "standard" },
      "updatedAt": { "type": "date" },
      "blocks": {
        "type": "nested",
        "properties": {
          "blockId": { "type": "keyword" },
          "pageId": { "type": "keyword" },
          "type": { "type": "keyword" },
          "content": { "type": "text", "analyzer": "standard" }
        }
      }
    }
  }
}'

    echo "Indexes initialized."