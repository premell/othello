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

type CreateGameRequestBody struct {
	TimeLimit     int
	TimeIncrement int
}

type GetGameRequestBody struct {
	GameID int
}

type GameState struct {
	GameState            string
	PlayerColor          int
	PlayerRemainingTime  int
	PreviousMoveNumber   int
	PreviousTargetSquare int
}

type Game struct {
	TimeLimit          int    `json:"time_limit,omitempty"`
	TimeIncrement      int    `json:"time_increment,omitempty"`
	GameStatus         string `json:"game_status,omitempty"`
	WhiteTimeRemaining int    `json:"white_time_remaining,omitempty"`
	BlackTimeRemaining int    `json:"black_time_remaining,omitempty"`
	GameStates         []GameState
}

type MovesData struct {
	TimeLimit          int `json:"time_limit,omitempty"`
	TimeIncrement      int `json:"time_increment,omitempty"`
	GameStatus         int `json:"game_status,omitempty"`
	WhiteTimeRemaining int `json:"white_time_remaining,omitempty"`
	BlackTimeRemaining int `json:"black_time_remaining,omitempty"`
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
	// router.POST("/game", pass)
	router.GET("/game/getGame", getGame)
	// router.GET("/game", getTime)

	router.NoRoute(func(c *gin.Context) {
		c.JSON(404, gin.H{"code": "PAGE_NOT_FOUND", "message": "Page not found"})
	})

	router.Run(":1011")

}

func createGame(c *gin.Context) {
	var requestBody CreateGameRequestBody

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
	query := `
    SELECT g.time_limit, g.time_increment, g.white_time_remaining, g.black_time_remaining, status.status_text, 
	moves.resulting_state, moves.target_square,
	moves.move_number,
	moves.player_color,
	moves.player_remaining_time
    FROM games AS g
    JOIN moves ON g.id = moves.game_id
    JOIN game_statuses AS status ON g.game_status = status.status_id
    WHERE g.id= ?
  `

	var requestBody GetGameRequestBody

	if err := c.BindJSON(&requestBody); err != nil {
		panic(err)
	}

	rows, err := DB.Query(query, requestBody.GameID)
	if err != nil {
		panic(err)
	}

	game := &Game{}
	for rows.Next() {
		state := &GameState{}
		err = rows.Scan(

			&game.TimeLimit, &game.TimeIncrement, &game.WhiteTimeRemaining, &game.BlackTimeRemaining, &game.GameStatus,
			&state.GameState,
			&state.PreviousTargetSquare,
			&state.PreviousMoveNumber,
			&state.PlayerColor,
			&state.PlayerRemainingTime,
		)
		if err != nil {
			panic(err)
		}
		game.GameStates = append(game.GameStates, *state)
	}

	fmt.Println("hejsan")
	fmt.Printf("%v", game)

	c.JSON(200, game)
}
