package main

import (
	"github.com/gin-gonic/gin"
	"database/sql"
  _ "github.com/go-sql-driver/mysql"
	// "net/http"
)

func main() {
	router := gin.Default()

	router.POST("/game", createGame)
	router.POST("/game", makeMove)
	router.POST("/game", revertMove)
	router.POST("/game", surrender)
	// router.POST("/game", pass)
	router.GET("/game", getGame)
	router.GET("/game", getTime)

	router.Run(":80")

db, err := sql.Open("mysql",
		"user:password@tcp(127.0.0.1:3306)/hello")
	if err != nil {
		log.Fatal(err)
	}
	defer db.Close()

}


// I need game time and color
func createGame(c *gin.Context) {
}

// I need game game id, color and move
func makeMove(c *gin.Context) {
}

// I need game game id and color
func surrender(c *gin.Context) {
}
