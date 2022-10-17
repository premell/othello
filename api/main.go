package main

import (
	"database/sql"
	"fmt"
	"log"
	    //"encoding/json"

	"github.com/gin-gonic/gin"
	_ "github.com/go-sql-driver/mysql"
	// "net/http"
)

var DB *sql.DB

type CreateGameData struct {
	TimeLimit     int
	TimeIncrement int
}

type GetGameData struct {
	GameID int
}

func main() {

	db, err := sql.Open("mysql",
		"root:hej.@tcp(127.0.0.1:3306)/othello")
	if err != nil {
		log.Fatal(err)
	}
	err = db.Ping()
	if err != nil {
		log.Fatal(err)
	}
	DB = db
	defer db.Close()

	router := gin.Default()

	router.POST("/game/createGame", createGame)
	// router.POST("/game", makeMove)
	// router.POST("/game", revertMove)
	// router.POST("/game", surrender)
	// // router.POST("/game", pass)
	router.GET("/game/getGame", getGame)
	// router.GET("/game", getTime)

	router.NoRoute(func(c *gin.Context) {
		c.JSON(404, gin.H{"code": "PAGE_NOT_FOUND", "message": "Page not found"})
	})

	router.Run(":1202")

}

func createGame(c *gin.Context) {
	var requestBody CreateGameData

	if err := c.BindJSON(&requestBody); err != nil {
		fmt.Print("this?")
		panic(err)
	}

	ins, err := DB.Prepare("INSERT INTO games (time_limit, time_increment, game_status, white_time_remaining, black_time_remaining) VALUES (?,?,?,?,?)")
	if err != nil {
		panic(err)
	}

	defer ins.Close()

	res, err := ins.Exec(
		requestBody.TimeLimit,
		requestBody.TimeIncrement,
		5,
		requestBody.TimeLimit,
		requestBody.TimeLimit,
	)

	rowsAffec, _ := res.RowsAffected()
	if err != nil || rowsAffec != 1 {
		panic(err)
	}
}

func getGame(c *gin.Context) {

	var requestBody GetGameData

	if err := c.BindJSON(&requestBody); err != nil {
		fmt.Print("this?")
		panic(err)
	}

	res, err := DB.Query("SELECT * FROM moves WHERE game_ID=?", requestBody.GameID)

	fmt.Println(res)

	if err != nil {
		panic(err)
	}

  c.JSON(200, res)
}

